#version 330 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec2 aTexCoords;

out vec3 FragPos;
out vec2 TexCoords;
out vec4 FragPosLightSpace;

uniform mat4 model;
uniform mat4 lightSpaceMatrix;

layout (std140) uniform Matrices
{
    mat4 projection;
    mat4 view;
};


void main()
{
    gl_Position = projection*view*model* vec4(aPos, 1.0);
    FragPos = vec3(model * vec4(aPos,1.0));
    TexCoords = aTexCoords;
    FragPosLightSpace = lightSpaceMatrix * vec4(FragPos,1.0);
}