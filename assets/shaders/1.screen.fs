#version 330 core
out vec4 FragColor;

in vec2 TexCoords;

uniform sampler2D sceneTexture;
uniform sampler2D bloomBlur;
uniform float exposure;

const float offset = 1.0/300.0;
void main()
{
    //vec2 offsets[9] = vec2[](
    //    vec2(-offset,  offset), // 左上
    //    vec2( 0.0f,    offset), // 正上
    //    vec2( offset,  offset), // 右上
    //    vec2(-offset,  0.0f),   // 左
    //    vec2( 0.0f,    0.0f),   // 中
    //    vec2( offset,  0.0f),   // 右
    //    vec2(-offset, -offset), // 左下
    //    vec2( 0.0f,   -offset), // 正下
    //    vec2( offset, -offset)  // 右下
    //);
    ////float kernel[9] = float[](
    ////    -1, -1, -1,
    ////    -1,  9, -1,
    ////    -1, -1, -1
    ////);
    ////float kernel[9] = float[](
    ////1.0 / 16, 2.0 / 16, 1.0 / 16,
    ////2.0 / 16, 4.0 / 16, 2.0 / 16,
    ////1.0 / 16, 2.0 / 16, 1.0 / 16  
    //float kernel[9] = float[](
    //0.0,0.0,0.0,
    //0.0,1.0,0.0,
    //0.0,0.0,0.0
    //);
    //vec3 sampleTex[9];
    //for(int i = 0; i < 9; i++)
    //{
    //    sampleTex[i] = vec3(texture(screenTexture, TexCoords.st + offsets[i]));
    //}
    //vec3 col = vec3(0.0);
    //for(int i = 0; i < 9; i++)
    //    col += sampleTex[i] * kernel[i];

    vec3 scene = texture(sceneTexture, TexCoords).rgb;
    vec3 bloom = texture(bloomBlur, TexCoords).rgb;
    vec3 color = scene + bloom;

    //vec3 hdrColor = texture(screenTexture, TexCoords).rgb;

    // 曝光色调映射
    vec3 mapped = 1.0 - exp(-color * exposure);
    //vec3 mapped = hdrColor;
    // Gamma
    //float gamma = 2.2;
    //mapped = pow(mapped, vec3(1.0 / gamma));

    FragColor = vec4(mapped, 1.0);
}