#version 330 core
out vec4 FragColor;

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
in vec3 Position;

uniform DirLight dirLight;
uniform PointLight pointLights;
uniform SpotLight spotLight;
uniform vec3 viewPos;
uniform float shininess;
uniform bool spotLightOpen;

uniform sampler2D texture_diffuse1;
uniform sampler2D texture_specular1;
uniform samplerCube skybox;

void main()
{
    vec3 I = normalize(Position - viewPos);
    vec3 N = normalize(Normal);
    vec3 R = reflect(I, N);
    // R = vec4(texture(skybox, R).rgb, 1.0);

    float ratio = 1.00/1.52;
    vec3 T = refract(I, N, ratio);

    float bias = 0.1;
    float scale = 0.9;
    float power = 1.5;
    float fresnel = bias + scale *pow(1.0-dot(-I,N),power);
    
    vec3 reflectColor = texture(skybox, R).rgb;
    vec3 refractColor = texture(skybox, T).rgb;
    vec3 finalColor = mix(refractColor, reflectColor, fresnel);
    FragColor = vec4(finalColor, 1.0);
}
