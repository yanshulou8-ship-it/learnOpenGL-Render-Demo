#ifndef CAMERA_H
#define CAMERA_H

#include <glad/glad.h>
#include <glm/glm.hpp>
#include <glm/gtc/matrix_transform.hpp>

// 定义相机的几种移动方式，用于统一输入方式，不依赖具体窗口系统
enum Camera_Movement {
    FORWARD,    // 前进
    BACKWARD,   // 后退
    LEFT,       // 左移
    RIGHT,      // 右移
    UP,         //上移   
    DOWN        //下移
};

// 默认相机参数
const float YAW = -90.0f;  // 初始偏航角
const float PITCH = 0.0f;   // 初始俯仰角
const float SPEED = 2.5f;   // 移动速度
const float SENSITIVITY = 0.1f;   // 鼠标灵敏度
const float ZOOM = 45.0f;  // 初始视野（FOV）


// 一个抽象的相机类，处理输入并计算对应的欧拉角、方向向量和视图矩阵
class Camera
{
public:
    // 相机属性
    glm::vec3 Position;   // 位置
    glm::vec3 Front;      // 前方向
    glm::vec3 Up;         // 上方向
    glm::vec3 Right;      // 右方向
    glm::vec3 WorldUp;    // 世界空间上方向

    // 欧拉角
    float Yaw;    // 偏航角（左右看）
    float Pitch;  // 俯仰角（上下看）

    // 相机可调参数
    float MovementSpeed;     // 移动速度
    float MouseSensitivity;  // 鼠标灵敏度
    float Zoom;              // 视野大小

    // 向量构造函数
    Camera(glm::vec3 position = glm::vec3(0.0f, 0.0f, 0.0f),
        glm::vec3 up = glm::vec3(0.0f, 1.0f, 0.0f),
        float yaw = YAW,
        float pitch = PITCH)
        : Front(glm::vec3(0.0f, 0.0f, -1.0f)),
        MovementSpeed(SPEED),
        MouseSensitivity(SENSITIVITY),
        Zoom(ZOOM)
    {
        Position = position;
        WorldUp = up;
        Yaw = yaw;
        Pitch = pitch;
        updateCameraVectors(); // 更新方向向量
    }

    // 标量构造函数
    Camera(float posX, float posY, float posZ,
        float upX, float upY, float upZ,
        float yaw, float pitch)
        : Front(glm::vec3(0.0f, 0.0f, -1.0f)),
        MovementSpeed(SPEED),
        MouseSensitivity(SENSITIVITY),
        Zoom(ZOOM)
    {
        Position = glm::vec3(posX, posY, posZ);
        WorldUp = glm::vec3(upX, upY, upZ);
        Yaw = yaw;
        Pitch = pitch;
        updateCameraVectors(); // 更新方向向量
    }

    // 使用欧拉角和 LookAt 矩阵计算并返回视图矩阵
    glm::mat4 GetViewMatrix()
    {
        return glm::lookAt(Position, Position + Front, Up);
    }

    // 处理键盘输入
    void ProcessKeyboard(Camera_Movement direction, float deltaTime)
    {
        float velocity = MovementSpeed * deltaTime;

        if (direction == FORWARD)
            Position += Front * velocity;   // 前进
        if (direction == BACKWARD)
            Position -= Front * velocity;   // 后退
        if (direction == LEFT)
            Position -= Right * velocity;   // 左移
        if (direction == RIGHT)
            Position += Right * velocity;   // 右移
        if (direction == UP)
            Position += Up * velocity;      //上移
        if (direction == DOWN)
            Position -= Up * velocity;      //下移
    }

    // 处理鼠标移动输入
    void ProcessMouseMovement(float xoffset, float yoffset, GLboolean constrainPitch = true)
    {
        // 应用鼠标灵敏度
        xoffset *= MouseSensitivity;
        yoffset *= MouseSensitivity;

        Yaw += xoffset;  // 更新偏航角
        Pitch += yoffset;  // 更新俯仰角

        // 限制俯仰角，防止画面翻转
        if (constrainPitch)
        {
            if (Pitch > 89.0f)
                Pitch = 89.0f;
            if (Pitch < -89.0f)
                Pitch = -89.0f;
        }

        // 根据新的欧拉角更新方向向量
        updateCameraVectors();
    }

    // 处理鼠标滚轮缩放
    void ProcessMouseScroll(float yoffset)
    {
        Zoom -= (float)yoffset;

        if (Zoom < 1.0f)
            Zoom = 1.0f;
        if (Zoom > 45.0f)
            Zoom = 45.0f;
    }

private:
    // 根据更新后的欧拉角重新计算前、右、上方向向量
    void updateCameraVectors()
    {
        // 计算新的前方向向量
        glm::vec3 front;
        front.x = cos(glm::radians(Yaw)) * cos(glm::radians(Pitch));
        front.y = sin(glm::radians(Pitch));
        front.z = sin(glm::radians(Yaw)) * cos(glm::radians(Pitch));

        Front = glm::normalize(front);

        // 重新计算右方向和上方向
        Right = glm::normalize(glm::cross(Front, WorldUp));
        Up = glm::normalize(glm::cross(Right, Front));
    }
};
#endif