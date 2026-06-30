#version 330 core
layout (location = 0)out vec4 FragColor;
layout (location = 1)out vec4 BrightColor;
in vec3 TexCoords; // 代表3D纹理坐标的方向向量
uniform samplerCube cubemap; // 立方体贴图的纹理采样器

void main()
{             
    FragColor = texture(cubemap, TexCoords);
    float brightness = dot(FragColor.rgb, vec3(0.2126, 0.7152, 0.0722));
    if(brightness > 0.8)
        BrightColor = vec4(FragColor.rgb, 1.0);
}
