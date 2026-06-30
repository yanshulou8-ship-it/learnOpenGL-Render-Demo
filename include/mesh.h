#ifndef MESH_H
#define MESH_H

#include <glad/glad.h> // 包含所有OpenGL类型声明

#include <glm/glm.hpp>
#include <glm/gtc/matrix_transform.hpp>

#include <shader.h>

#include <string>
#include <vector>
using namespace std;

unsigned int defaultDiffuseTex;
unsigned int defaultNormalTex;
unsigned int defaultSpecularTex;
unsigned int defaultHeightTex;

// 最多4根骨骼影响同一个顶点（骨骼动画用）
#define MAX_BONE_INFLUENCE 4

// 顶点结构体：存储单个顶点的所有数据
struct Vertex {
    // 顶点位置
    glm::vec3 Position;
    // 顶点法线
    glm::vec3 Normal;
    // 纹理坐标
    glm::vec2 TexCoords;
    // 切线
    glm::vec3 Tangent;
    // 副切线
    glm::vec3 Bitangent;
    // 影响该顶点的骨骼ID
    int m_BoneIDs[MAX_BONE_INFLUENCE];
    // 每根骨骼的影响权重
    float m_Weights[MAX_BONE_INFLUENCE];
};

// 纹理结构体
struct Texture {
    unsigned int id;   // 纹理ID
    string type;       // 纹理类型（漫反射、高光等）
    string path;       // 纹理文件路径
};

// 网格类：管理模型的顶点、索引、纹理和渲染
class Mesh {
public:
    // 网格数据
    vector<Vertex>       vertices;   // 顶点集合
    vector<unsigned int> indices;    // 索引集合
    vector<Texture>      textures;   // 纹理集合
    unsigned int VAO;                // 顶点数组对象

    // 构造函数：初始化网格数据
    Mesh(vector<Vertex> vertices, vector<unsigned int> indices, vector<Texture> textures)
    {
        this->vertices = vertices;
        this->indices = indices;
        this->textures = textures;

        // 数据准备完成，设置顶点缓冲与属性指针
        setupMesh();
    }

    // 渲染网格
    void Draw(Shader& shader)
    {

        glActiveTexture(GL_TEXTURE0);  glBindTexture(GL_TEXTURE_2D, 0);
        glActiveTexture(GL_TEXTURE1);  glBindTexture(GL_TEXTURE_2D, 0);
        glActiveTexture(GL_TEXTURE2);  glBindTexture(GL_TEXTURE_2D, 0);
        glActiveTexture(GL_TEXTURE3);  glBindTexture(GL_TEXTURE_2D, 0);

        // 绑定对应的纹理
        unsigned int diffuseNr = 1;
        unsigned int specularNr = 1;
        unsigned int normalNr = 1;
        unsigned int heightNr = 1;
        for (unsigned int i = 0; i < textures.size(); i++)
        {
            glActiveTexture(GL_TEXTURE0 + i); // 绑定前先激活对应的纹理单元

            // 获取纹理编号（对应 shader 中的 diffuse_textureN）
            string number;
            string name = textures[i].type;
            if (name == "texture_diffuse")
                number = std::to_string(diffuseNr++);
            else if (name == "texture_specular")
                number = std::to_string(specularNr++);
            else if (name == "texture_normal")
                number = std::to_string(normalNr++);
            else if (name == "texture_height")
                number = std::to_string(heightNr++);

            // 设置采样器使用正确的纹理单元
            glUniform1i(glGetUniformLocation(shader.ID, (name + number).c_str()), i);
            // 绑定纹理
            glBindTexture(GL_TEXTURE_2D, textures[i].id);
        }

        glUniform1i(glGetUniformLocation(shader.ID, "hasNormalMap"), (normalNr > 1));

        // 绘制网格
        glBindVertexArray(VAO);
        glDrawElements(GL_TRIANGLES, static_cast<unsigned int>(indices.size()), GL_UNSIGNED_INT, 0);
        glBindVertexArray(0);

        // 渲染完成后恢复默认状态，是良好的编程习惯
        glActiveTexture(GL_TEXTURE0);
    }

private:
    // 渲染缓冲对象
    unsigned int VBO, EBO;

    // 初始化所有缓冲对象与顶点数组
    void setupMesh()
    {
        // 创建顶点数组、顶点缓冲、索引缓冲
        glGenVertexArrays(1, &VAO);
        glGenBuffers(1, &VBO);
        glGenBuffers(1, &EBO);

        glBindVertexArray(VAO);

        // 将顶点数据传入顶点缓冲
        glBindBuffer(GL_ARRAY_BUFFER, VBO);
        // 结构体的好处：内存布局是连续的
        // 可以直接传递结构体指针，完美对应 glm::vec3/vec2 数据
        glBufferData(GL_ARRAY_BUFFER, vertices.size() * sizeof(Vertex), &vertices[0], GL_STATIC_DRAW);

        // 将索引数据传入索引缓冲
        glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, EBO);
        glBufferData(GL_ELEMENT_ARRAY_BUFFER, indices.size() * sizeof(unsigned int), &indices[0], GL_STATIC_DRAW);

        // 设置顶点属性指针，告诉GPU如何解析顶点数据

        // 顶点位置 属性
        glEnableVertexAttribArray(0);
        glVertexAttribPointer(0, 3, GL_FLOAT, GL_FALSE, sizeof(Vertex), (void*)0);
        // 顶点法线 属性
        glEnableVertexAttribArray(1);
        glVertexAttribPointer(1, 3, GL_FLOAT, GL_FALSE, sizeof(Vertex), (void*)offsetof(Vertex, Normal));
        // 顶点纹理坐标 属性
        glEnableVertexAttribArray(2);
        glVertexAttribPointer(2, 2, GL_FLOAT, GL_FALSE, sizeof(Vertex), (void*)offsetof(Vertex, TexCoords));
        // 顶点切线 属性
        glEnableVertexAttribArray(3);
        glVertexAttribPointer(3, 3, GL_FLOAT, GL_FALSE, sizeof(Vertex), (void*)offsetof(Vertex, Tangent));
        // 顶点副切线 属性
        glEnableVertexAttribArray(4);
        glVertexAttribPointer(4, 3, GL_FLOAT, GL_FALSE, sizeof(Vertex), (void*)offsetof(Vertex, Bitangent));
        // 骨骼ID 属性（使用整数指针）
        glEnableVertexAttribArray(5);
        glVertexAttribIPointer(5, 4, GL_INT, sizeof(Vertex), (void*)offsetof(Vertex, m_BoneIDs));

        // 骨骼权重 属性
        glEnableVertexAttribArray(6);
        glVertexAttribPointer(6, 4, GL_FLOAT, GL_FALSE, sizeof(Vertex), (void*)offsetof(Vertex, m_Weights));

        // 解绑VAO
        glBindVertexArray(0);
    }
};
void InitDefaultTextures()
{
    // 纯白漫反射
    GLubyte white[] = {255,255,255,255};
    glGenTextures(1, &defaultDiffuseTex);
    glBindTexture(GL_TEXTURE_2D, defaultDiffuseTex);
    glTexImage2D(GL_TEXTURE_2D,0,GL_RGBA,1,1,0,GL_RGBA,GL_UNSIGNED_BYTE,white);
    glTexParameteri(GL_TEXTURE_2D,GL_TEXTURE_MIN_FILTER,GL_LINEAR);
    glTexParameteri(GL_TEXTURE_2D,GL_TEXTURE_MAG_FILTER,GL_LINEAR);

    // 默认平坦法线 128,128,255
    GLubyte normalDef[] = {128,128,255,255};
    glGenTextures(1, &defaultNormalTex);
    glBindTexture(GL_TEXTURE_2D, defaultNormalTex);
    glTexImage2D(GL_TEXTURE_2D,0,GL_RGBA,1,1,0,GL_RGBA,GL_UNSIGNED_BYTE,normalDef);
    glTexParameteri(GL_TEXTURE_2D,GL_TEXTURE_MIN_FILTER,GL_LINEAR);
    glTexParameteri(GL_TEXTURE_2D,GL_TEXTURE_MAG_FILTER,GL_LINEAR);

    // 高光/高度默认黑色
    GLubyte black[] = {0,0,0,255};
    glGenTextures(1, &defaultSpecularTex);
    glBindTexture(GL_TEXTURE_2D, defaultSpecularTex);
    glTexImage2D(GL_TEXTURE_2D,0,GL_RGBA,1,1,0,GL_RGBA,GL_UNSIGNED_BYTE,black);
    glTexParameteri(GL_TEXTURE_2D,GL_TEXTURE_MIN_FILTER,GL_LINEAR);
    glTexParameteri(GL_TEXTURE_2D,GL_TEXTURE_MAG_FILTER,GL_LINEAR);

    glGenTextures(1, &defaultHeightTex);
    glBindTexture(GL_TEXTURE_2D, defaultHeightTex);
    glTexImage2D(GL_TEXTURE_2D,0,GL_RGBA,1,1,0,GL_RGBA,GL_UNSIGNED_BYTE,black);
    glTexParameteri(GL_TEXTURE_2D,GL_TEXTURE_MIN_FILTER,GL_LINEAR);
    glTexParameteri(GL_TEXTURE_2D,GL_TEXTURE_MAG_FILTER,GL_LINEAR);

    glBindTexture(GL_TEXTURE_2D, 0);
}

#endif