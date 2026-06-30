#version 330 core
layout (location = 0) out vec4 FragColor;
layout (location = 1) out vec4 BrightColor;

in vec2 TexCoords;

uniform sampler2D gPosition;   // 观察空间位置
uniform sampler2D gNormal;     // 观察空间法线
uniform sampler2D gAlbedoSpec; // 颜色(RGB) + 高光(A)
uniform sampler2D ssao;        // SSAO 遮蔽图

// 阴影和反射所需要的数据
uniform samplerCube depthMap;
uniform samplerCube skybox;
uniform float far_plane;
uniform float shininess;
uniform mat4 inverseView; // 关键：用来把 View 空间转回 World 空间计算阴影和天空盒

struct PointLight {
    vec3 position; // 观察空间位置
    float constant;
    float linear;
    float quadratic;
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};
uniform PointLight pointLights;

struct SpotLight {
    vec3 position;
    vec3 direction;
    float cutOff;
    float outerCutOff;
    float constant;
    float linear;
    float quadratic;
    vec3 ambient;
    vec3 diffuse;
    vec3 specular; 
};
uniform SpotLight spotLight;
uniform bool spotLightOpen;

// 继承你原本极为精细的 64 采样点光源 PCF 偏移数组
uniform vec3 sampleOffsetDirections[64];

// 高级万向阴影计算（需要转回世界空间）
float cubeShadowCalculation(vec3 fragPosView, vec3 normalView) {
    // 转换到世界空间以进行立方体贴图采样
    vec3 fragPosWorld = vec3(inverseView * vec4(fragPosView, 1.0));
    vec3 lightPosWorld = vec3(inverseView * vec4(pointLights.position, 1.0));
    vec3 normalWorld = normalize(mat3(inverseView) * normalView);

    vec3 fragToLight = fragPosWorld - lightPosWorld;
    float currentDepth = length(fragToLight);
    
    float shadow = 0.0;
    float bias = max(0.05 * (1.0 - dot(normalWorld, normalize(-fragToLight))), 0.005);
    int samples = 64;
    float diskRadius = 0.05;
    
    for(int i = 0; i < samples; ++i) {
        float closestDepth = texture(depthMap, fragToLight + sampleOffsetDirections[i] * diskRadius).r;
        closestDepth *= far_plane;   
        if(currentDepth - bias > closestDepth)
            shadow += 1.0;
    }
    shadow /= float(samples);
    return shadow;
}

void main() {             
    // 从 G-Buffer 提取信息
    vec3 fragPos = texture(gPosition, TexCoords).rgb;
    vec3 normal = texture(gNormal, TexCoords).rgb;
    vec3 albedo = texture(gAlbedoSpec, TexCoords).rgb;
    float specularIntensity = texture(gAlbedoSpec, TexCoords).a;
    float AmbientOcclusion = texture(ssao, TexCoords).r; // 融合 SSAO

    if (length(normal) < 0.1) {
        discard; 
    }

    vec3 viewDir = normalize(-fragPos); // 观察空间中相机在 (0,0,0)

    // 计算高品质 64 采样点光源阴影
    float shadow = cubeShadowCalculation(fragPos, normal);

    // ---- 1. 点光源 Blinn-Phong 计算 ----
    vec3 lightDir = normalize(pointLights.position - fragPos);
    // 漫反射
    float diff = max(dot(normal, lightDir), 0.0);
    // 镜面高光
    vec3 halfwayDir = normalize(lightDir + viewDir);
    float spec = pow(max(dot(normal, halfwayDir), 0.0), shininess);
    // 衰减
    float distance = length(pointLights.position - fragPos);
    float attenuation = 1.0 / (pointLights.constant + pointLights.linear * distance + pointLights.quadratic * (distance * distance));    
    
    // 结合 SSAO 到环境光中
    vec3 ambient = pointLights.ambient * albedo * AmbientOcclusion; 
    vec3 diffuse = pointLights.diffuse * diff * albedo;
    vec3 specular = pointLights.specular * spec * specularIntensity;

    vec3 result = (ambient + (1.0 - shadow) * (diffuse + specular)) * attenuation;

    // ---- 2. 聚光灯 (SpotLight) 计算 ----
    if (spotLightOpen) {
        vec3 spotLightDir = normalize(spotLight.position - fragPos);
        float spotDiff = max(dot(normal, spotLightDir), 0.0);
        
        vec3 spotReflectDir = reflect(-spotLightDir, normal);
        float spotSpec = pow(max(dot(viewDir, spotReflectDir), 0.0), shininess);
        
        float spotDist = length(spotLight.position - fragPos);
        float spotAtten = 1.0 / (spotLight.constant + spotLight.linear * spotDist + spotLight.quadratic * (spotDist * spotDist));    
        
        float theta = dot(spotLightDir, normalize(-spotLight.direction)); 
        float epsilon = spotLight.cutOff - spotLight.outerCutOff;
        float intensity = clamp((theta - spotLight.outerCutOff) / epsilon, 0.0, 1.0);

        vec3 spotAmbient = spotLight.ambient * albedo;
        vec3 spotDiffuse = spotLight.diffuse * spotDiff * albedo;
        vec3 spotSpecular = spotLight.specular * spotSpec * specularIntensity;

        result += (spotAmbient + spotDiffuse + spotSpecular) * spotAtten * intensity;
    }

    // ---- 3. 天空盒镜面环境反射 (Skybox Reflection) ----
    vec3 I = normalize(fragPos); // 观察空间视线方向
    vec3 R = reflect(I, normal);
    vec3 R_world = mat3(inverseView) * R; // 必须转回世界空间采样天空盒
    vec3 reflectColor = texture(skybox, R_world).rgb;

    float smoothness = specularIntensity * 0.1; // 继承你原先的平滑度逻辑
    result = mix(result, reflectColor, smoothness);
    
    // 输出最终颜色
    FragColor = vec4(result, 1.0);
    
    // 完美的 Bloom 兼容阈值提取
    float brightness = dot(FragColor.rgb, vec3(0.2126, 0.7152, 0.0722));
    if(brightness > 0.8)
        BrightColor = vec4(FragColor.rgb, 1.0);
    else
        BrightColor = vec4(0.0, 0.0, 0.0, 1.0);
}