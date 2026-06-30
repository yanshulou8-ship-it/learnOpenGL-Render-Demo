#version 330 core
layout (location = 0)out vec4 FragColor;
layout (location = 1)out vec4 BrightColor;

struct DirLight {
    vec3 direction;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};
struct PointLight {
    vec3 position;

    float constant;
    float linear;
    float quadratic;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};  
struct SpotLight{
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
#define NR_POINT_LIGHTS 1

in vec2 TexCoords;
in vec3 Normal;
in vec3 FragPos;
in mat3 TBN;
//in vec4 FragPosLightSpace;

uniform DirLight dirLight;
uniform PointLight pointLights;
uniform SpotLight spotLight;
uniform vec3 viewPos;
uniform float shininess;
uniform float height_scale;
uniform bool spotLightOpen;

uniform sampler2D texture_diffuse1;
uniform sampler2D texture_specular1;
uniform sampler2D texture_normal1;
uniform sampler2D texture_height1;
uniform samplerCube depthMap;
//uniform sampler2D shadowMap;
uniform samplerCube skybox;
uniform float far_plane;

//vec3 sampleOffsetDirections[20] = vec3[]
//(
//   vec3( 1,  1,  1), vec3( 1, -1,  1), vec3(-1, -1,  1), vec3(-1,  1,  1), 
//   vec3( 1,  1, -1), vec3( 1, -1, -1), vec3(-1, -1, -1), vec3(-1,  1, -1),
//   vec3( 1,  1,  0), vec3( 1, -1,  0), vec3(-1, -1,  0), vec3(-1,  1,  0),
//   vec3( 1,  0,  1), vec3(-1,  0,  1), vec3( 1,  0, -1), vec3(-1,  0, -1),
//   vec3( 0,  1,  1), vec3( 0, -1,  1), vec3( 0, -1, -1), vec3( 0,  1, -1)
//);

vec3 sampleOffsetDirections[64] = vec3[]
(
vec3( 0.25,  0.90,  0.35),vec3(-0.30,  0.85,  0.40),vec3( 0.40, -0.80,  0.30),vec3(-0.35, -0.75,  0.45),
vec3( 0.85,  0.30,  0.40),vec3(-0.80,  0.35,  0.30),vec3( 0.75, -0.40,  0.35),vec3(-0.90, -0.25,  0.30),
vec3( 0.15,  0.35,  0.90),vec3(-0.20,  0.30,  0.85),vec3( 0.25, -0.35,  0.80),vec3(-0.15, -0.30,  0.90),
vec3( 0.35,  0.15,  0.85),vec3(-0.30,  0.20,  0.80),vec3( 0.45, -0.15,  0.75),vec3(-0.40, -0.20,  0.85),
vec3( 0.60,  0.60,  0.50),vec3(-0.55,  0.65,  0.45),vec3( 0.65, -0.55,  0.40),vec3(-0.60, -0.60,  0.50),
vec3( 0.50,  0.10,  0.80),vec3(-0.45,  0.15,  0.75),vec3( 0.55, -0.10,  0.70),vec3(-0.50, -0.15,  0.80),
vec3( 0.10,  0.60,  0.70),vec3(-0.15,  0.55,  0.65),vec3( 0.20, -0.50,  0.70),vec3(-0.10, -0.60,  0.65),
vec3( 0.70,  0.10,  0.60),vec3(-0.65,  0.15,  0.55),vec3( 0.60, -0.20,  0.50),vec3(-0.70, -0.10,  0.60),
vec3( 0.25,  0.25,  0.90),vec3(-0.20,  0.30,  0.85),vec3( 0.30, -0.25,  0.80),vec3(-0.25, -0.20,  0.90),
vec3( 0.80,  0.10,  0.45),vec3(-0.75,  0.15,  0.40),vec3( 0.70, -0.20,  0.35),vec3(-0.85, -0.10,  0.45),
vec3( 0.10,  0.80,  0.45),vec3(-0.15,  0.75,  0.40),vec3( 0.20, -0.70,  0.35),vec3(-0.10, -0.85,  0.45),
vec3( 0.45,  0.45,  0.60),vec3(-0.40,  0.50,  0.55),vec3( 0.50, -0.40,  0.50),vec3(-0.45, -0.45,  0.60),
vec3( 0.30,  0.70,  0.45),vec3(-0.25,  0.65,  0.40),vec3( 0.35, -0.60,  0.35),vec3(-0.30, -0.70,  0.45),
vec3( 0.55,  0.35,  0.60),vec3(-0.50,  0.40,  0.55),vec3( 0.60, -0.30,  0.50),vec3(-0.55, -0.35,  0.60),
vec3( 0.15,  0.15,  0.95),vec3(-0.10,  0.20,  0.90),vec3( 0.20, -0.15,  0.85),vec3(-0.15, -0.10,  0.95),
vec3( 0.90,  0.15,  0.35),vec3(-0.85,  0.20,  0.30),vec3( 0.80, -0.25,  0.25),vec3(-0.95, -0.15,  0.35)
);

vec3 CalcDirLight(DirLight light, vec3 normal, vec3 viewDir,float shadow, vec2 texCoords);
vec3 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir ,float shadow, vec2 texCoords);
vec3 CalcSpotLight(SpotLight light, vec3 normal, vec3 fragPos, vec3 viewDir, vec2 texCoords);
vec2 ParallaxMapping(vec2 texCoords, vec3 viewDir);
//float ShadowCalculation(vec4 fragPosLightSpace);
float cubeShadowCalculation(vec3 fragPos);
void main()
{
    vec3 viewDirWorld = viewPos - FragPos;
    vec3 viewDirTangent = normalize(transpose(TBN )*viewDirWorld);
    vec2 texCoordsParallax = ParallaxMapping(TexCoords, viewDirTangent);

    vec3 normalTangent = texture(texture_normal1, texCoordsParallax).rgb;
    normalTangent = normalize(normalTangent * 2.0 - 1.0);    

    vec3 normalWorld = normalize(TBN * normalTangent);


    vec3 I = normalize(FragPos - viewPos);
    vec3 N = normalize(normalWorld);
    vec3 R = reflect(I, N);

    vec3 reflectColor = texture(skybox, R).rgb;

    // 属性
    vec3 norm = normalize(normalWorld);
    vec3 viewDir = normalize(viewPos - FragPos);
    // 计算阴影值
    vec3 parallaxWorldPos = FragPos + normalWorld * (1.0 - texture(texture_height1, texCoordsParallax).r) * height_scale;
    float shadow = cubeShadowCalculation(parallaxWorldPos); 
    //float shadow = cubeShadowCalculation(FragPos); 
    //shadow = 0.0;
    // 第一阶段：定向光照
    //vec3 result = CalcDirLight(dirLight, norm, viewDir,shadow);
    
    // 第二阶段：点光源
    //for(int i = 0; i < NR_POINT_LIGHTS; i++)
    //   result += CalcPointLight(pointLights, norm, FragPos, viewDir);  
    vec3 result = CalcPointLight(pointLights, norm, FragPos, viewDir,shadow, texCoordsParallax);
    // 第三阶段：聚光
    if (spotLightOpen)
        {
        result += CalcSpotLight(spotLight, norm, FragPos, viewDir, texCoordsParallax);
        }
    
    
    float smoothness = texture(texture_specular1,texCoordsParallax).r*0.1;
    result = mix(result,reflectColor,smoothness);
    
    
    
    FragColor = vec4(result, 1.0);
    //FragColor = vec4(vec3(gl_FragCoord.z),1);
    //FragColor = texture(texture_height1,TexCoords);
    float brightness = dot(FragColor.rgb, vec3(0.2126, 0.7152, 0.0722));
    if(brightness > 0.8)
        BrightColor = vec4(FragColor.rgb, 1.0);
}
//日光
vec3 CalcDirLight(DirLight light,vec3 normal,vec3 viewDir, float shadow ,vec2 texCoords)
{
    vec3 lightDir = normalize(-light.direction);
    // 漫反射着色
    float diff = max(dot(normal, lightDir), 0.0);
    // 镜面光着色
    vec3 halfwayDir = normalize(lightDir + viewDir);
    float spec = pow(max(dot(normal, halfwayDir), 0.0), shininess);
    // 合并结果
    vec3 ambient  = light.ambient  * vec3(texture(texture_diffuse1, texCoords));
    vec3 diffuse  = light.diffuse  * diff * vec3(texture(texture_diffuse1, texCoords));
    vec3 specular = light.specular * spec * vec3(texture(texture_specular1, texCoords));
    return (ambient + (1.0 - shadow) * (diffuse + specular));
}
//点光源
vec3 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir,float shadow, vec2 texCoords)
{
    vec3 lightDir = normalize(light.position - fragPos);
    // 漫反射着色
    float diff = max(dot(normal, lightDir), 0.0);
    // 镜面光着色
    vec3 halfwayDir = normalize(lightDir + viewDir);
    float spec = pow(max(dot(normal, halfwayDir), 0.0), shininess);
    // 衰减
    float distance    = length(light.position - fragPos);
    float attenuation = 1.0 / (light.constant + light.linear * distance + 
                 light.quadratic * (distance * distance));    
    // 合并结果
    vec3 ambient  = light.ambient  * vec3(texture(texture_diffuse1, texCoords));
    vec3 diffuse  = light.diffuse  * diff * vec3(texture(texture_diffuse1, texCoords));
    vec3 specular = light.specular * spec * vec3(texture(texture_specular1, texCoords));
    ambient  *= attenuation;
    diffuse  *= attenuation;
    specular *= attenuation;
    return (ambient + (1.0 - shadow) * (diffuse + specular));
}

//聚光灯
vec3 CalcSpotLight(SpotLight light, vec3 normal, vec3 fragPos, vec3 viewDir, vec2 texCoords){
    //漫反射
    vec3 lightDir = normalize(light.position-fragPos);
    float diff = max(dot(normal,lightDir),0.0);
    //高光
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), shininess);
    float distance = length(light.position - fragPos);
    float attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic * (distance * distance));    
    //聚光灯强度
    float theta = dot(lightDir, normalize(-light.direction)); 
    float epsilon = light.cutOff - light.outerCutOff;
    float intensity = clamp((theta - light.outerCutOff) / epsilon, 0.0, 1.0);

     vec3 ambient = light.ambient * vec3(texture(texture_diffuse1, texCoords));
    vec3 diffuse = light.diffuse * diff * vec3(texture(texture_diffuse1, texCoords));
    vec3 specular = light.specular * spec * vec3(texture(texture_specular1, texCoords));

    ambient *= attenuation * intensity;
    diffuse *= attenuation * intensity;
    specular *= attenuation * intensity;
    return (ambient + diffuse + specular);
}
//float ShadowCalculation(vec4 fragPosLightSpace)
//{
//    // 执行透视除法
//    vec3 lightDir = normalize(-dirLight.direction);
//    vec3 projCoords = fragPosLightSpace.xyz / fragPosLightSpace.w;
//    projCoords = projCoords * 0.5 + 0.5;
//    float closestDepth = texture(shadowMap, projCoords.xy).r;
//    float currentDepth = projCoords.z;
//    float bias = max(0.01 * (1.0 - dot(Normal, lightDir)), 0.005);
//    float shadow = 0.0;
//    vec2 texelSize = 1.0 / textureSize(shadowMap, 0);
//    for(int x = -1; x <= 1; ++x)
//    {
//        for(int y = -1; y <= 1; ++y)
//        {
//            float pcfDepth = texture(shadowMap, projCoords.xy + vec2(x, y) * texelSize).r; 
//            shadow += currentDepth - bias > pcfDepth ? 1.0 : 0.0;        
//        }    
//    }
//    shadow /= 9.0;
//    if(projCoords.z > 1.0)
//        shadow = 0.0;
//    return shadow;
//}
float cubeShadowCalculation(vec3 fragPos)
{
    vec3 fragToLight = fragPos - pointLights.position;
    //float closestDepth = texture(depthMap,fragToLight).r;
    //
    //closestDepth *= far_plane;
    //float currentDepth = length(fragToLight);
    //float bias = 0.05; 
    //float shadow = currentDepth -  bias > closestDepth ? 1.0 : 0.0;
    float shadow = 0.0;
    float bias = max(0.05 * (1.0 - dot(normalize(Normal), normalize(-fragToLight))), 0.005);
    int samples = 64;
    float diskRadius = 0.05;
    float currentDepth = length(fragToLight);
    for(int i = 0; i < samples; ++i)
{
    float closestDepth = texture(depthMap, fragToLight + sampleOffsetDirections[i] * diskRadius).r;
    closestDepth *= far_plane;   // Undo mapping [0;1]
    if(currentDepth - bias > closestDepth)
        shadow += 1.0;
}
    shadow /= float(samples);
    return shadow;
}
vec2 ParallaxMapping(vec2 texCoords, vec3 viewDir)
{ 
    // number of depth layers
    const float minLayers = 10;
    const float maxLayers = 20;
    float numLayers = mix(maxLayers, minLayers, abs(dot(vec3(0.0, 0.0, 1.0), viewDir)));  
    // calculate the size of each layer
    float layerDepth = 1.0 / numLayers;
    // depth of current layer
    float currentLayerDepth = 0.0;
    // the amount to shift the texture coordinates per layer (from vector P)
    vec2 P = vec2(viewDir.x, -viewDir.y) / viewDir.z * height_scale; 
    vec2 deltaTexCoords = P / numLayers;
  
    // get initial values
    vec2  currentTexCoords     = texCoords;
    float currentDepthMapValue = texture(texture_height1, currentTexCoords).r;
      
    while(currentLayerDepth < currentDepthMapValue)
    {
        // shift texture coordinates along direction of P
        currentTexCoords -= deltaTexCoords;
        // get depthmap value at current texture coordinates
        currentDepthMapValue = texture(texture_height1, currentTexCoords).r;  
        // get depth of next layer
        currentLayerDepth += layerDepth;  
    }
    
    // -- parallax occlusion mapping interpolation from here on
    // get texture coordinates before collision (reverse operations)
    vec2 prevTexCoords = currentTexCoords + deltaTexCoords;

    // get depth after and before collision for linear interpolation
    float afterDepth  = currentDepthMapValue - currentLayerDepth;
    float beforeDepth = texture(texture_height1, prevTexCoords).r - currentLayerDepth + layerDepth;
 
    // interpolation of texture coordinates
    float weight = afterDepth / (afterDepth - beforeDepth);
    vec2 finalTexCoords = prevTexCoords * weight + currentTexCoords * (1.0 - weight);

    return finalTexCoords;
}