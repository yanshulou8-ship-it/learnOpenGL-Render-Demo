#version 330 core
layout (location = 0) out vec4 gPosition;
layout (location = 1) out vec4 gNormal;
layout (location = 2) out vec4 gAlbedoSpec;

in vec3 FragPos;      // 已经在 View 空间
in vec2 TexCoords;
in vec3 Normal;        // 已经在 View 空间
in mat3 TBN;           // TBN 向量也转到了 View 空间下

uniform sampler2D texture_diffuse1;
uniform sampler2D texture_specular1;
uniform sampler2D texture_normal1;
uniform sampler2D texture_height1;

uniform float height_scale;

vec2 ParallaxMapping(vec2 texCoords, vec3 viewDir) { 
    const float minLayers = 10;
    const float maxLayers = 20;
    float numLayers = mix(maxLayers, minLayers, abs(dot(vec3(0.0, 0.0, 1.0), viewDir)));  
    float layerDepth = 1.0 / numLayers;
    float currentLayerDepth = 0.0;
    
    // 视差位移向量
    vec2 P = vec2(viewDir.x, -viewDir.y) / viewDir.z * height_scale; 
    vec2 deltaTexCoords = P / numLayers;
  
    vec2  currentTexCoords     = texCoords;
    float currentDepthMapValue = texture(texture_height1, currentTexCoords).r;
      
    while(currentLayerDepth < currentDepthMapValue) {
        currentTexCoords -= deltaTexCoords;
        currentDepthMapValue = texture(texture_height1, currentTexCoords).r;  
        currentLayerDepth += layerDepth;  
    }
    
    vec2 prevTexCoords = currentTexCoords + deltaTexCoords;
    float afterDepth  = currentDepthMapValue - currentLayerDepth;
    float beforeDepth = texture(texture_height1, prevTexCoords).r - currentLayerDepth + layerDepth;
 
    float weight = afterDepth / (afterDepth - beforeDepth);
    vec2 finalTexCoords = prevTexCoords * weight + currentTexCoords * (1.0 - weight);

    return finalTexCoords;
}

void main() {
    // 1. 计算切线空间下的视线方向，用于视差映射
    // 注意：这里的 TBN 是从世界/观察空间转到切线空间的
    mat3 tangentToView = TBN; 
    mat3 viewToTangent = transpose(TBN);
    vec3 viewDirTangent = normalize(viewToTangent * (-FragPos)); // 观察空间中相机在 (0,0,0)
    
    // 2. 执行视差映射，获取纠正后的纹理坐标
    vec2 texCoordsParallax = ParallaxMapping(TexCoords, viewDirTangent);
    
    // 如果视差坐标越界，抛弃该片段（边缘裁剪）
    if(texCoordsParallax.x > 1.0 || texCoordsParallax.y > 1.0 || texCoordsParallax.x < 0.0 || texCoordsParallax.y < 0.0)
        discard;

    // 3. 从法线贴图采样，并通过 TBN 转换到 View 空间
    vec3 normalTangent = texture(texture_normal1, texCoordsParallax).rgb;
    normalTangent = normalize(normalTangent * 2.0 - 1.0);    
    vec3 normalView = normalize(TBN * normalTangent);

    // 4. 计算视差修正后的实际 Fragment 位置
    vec3 parallaxViewPos = FragPos + normalView * (1.0 - texture(texture_height1, texCoordsParallax).r) * height_scale;

    // 5. 写入 G-Buffer
    gPosition = vec4(parallaxViewPos, 1.0);
    gNormal = vec4(normalView, 1.0);
    gAlbedoSpec.rgb = texture(texture_diffuse1, texCoordsParallax).rgb;
    gAlbedoSpec.a = texture(texture_specular1, texCoordsParallax).r;
}