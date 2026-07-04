#version 330 core
out vec4 FragColor;
in vec3 FragPos;
in vec3 Normal;
in vec2 TexCoords;

uniform vec3 viewPos;

uniform vec3 albedo;
uniform float metallic;
uniform float roughness;
uniform float ao;

uniform vec3 lightPositions[4];
uniform vec3 lightColors[4];

float PI = 3.14159265359;

vec3 fresnelSchlick(float cosTheta,vec3 F0);
float DistributionGGX(vec3 N,vec3 H,float roughness);
float GeometrySchlickGGX(float NdotV,float roughness);
float GeometrySmith(vec3 N,vec3 V,vec3 L,float roughness);

void main(){
		// Placeholder for PBR shading calculations
	vec3 N = normalize(Normal);
	vec3 V = normalize(viewPos - FragPos);

	vec3 Lo = vec3(0.0);
	for(int i = 0;i<4;++i)
	{
		//计算w0和半程向量
		vec3 L = normalize(lightPositions[i]-FragPos);
		vec3 H = normalize(V + L);
		//计算光照衰减
		float distance = length(lightPositions[i]-FragPos);
		float attenuation = 1.0 / (distance * distance);
		vec3 irradiance = lightColors[i] * attenuation;
		//计算FDG项
		vec3 F0 = vec3(0.04);
		F0 = mix(F0,albedo,metallic);
		vec3 F = fresnelSchlick(max(dot(H,V),0.0),F0);
		float NDF = DistributionGGX(N,H,roughness);
		float G = GeometrySmith(N,V,L,roughness);
		//计算BRDF
		vec3 num = NDF * G * F;
		float denom = 4.0 * max(dot(N,V),0.0) * max(dot(N,L),0.0)+0.001;
		vec3 specular = num / denom;

		vec3 kS = F;
		vec3 kD = vec3(1.0) - kS;

		kD *= 1.0 - metallic;

		float NdotL	= max(dot(N,L),0.0);
		Lo += (kD * albedo / PI + specular) * irradiance * NdotL;


	}
	vec3 ambient = vec3(0.03) * albedo * ao;
	vec3 color = ambient + Lo;
	//伽马矫正
	color = color / (color + vec3(1.0));
	color = pow(color,vec3(1.0/2.2));
	FragColor = vec4(color,1.0);
}

vec3 fresnelSchlick(float cosTheta,vec3 F0)
{
	return F0 + (1.0-F0)*pow(clamp(1.0 - cosTheta,0.0,1.0),5.0);

}

float DistributionGGX(vec3 N,vec3 H,float roughness)
{
	float a = roughness*roughness;
	float a2 = a*a;
	
	float NdotH = max(dot(N,H),0.0);
	float NdotH2 = NdotH*NdotH;

	float num = a2;
	float denom = (NdotH2*(a2-1.0)+1.0);
	denom = PI * denom*denom;
	
	return num / denom;
}

float GeometrySchlickGGX(float NdotV,float roughness)
{
	float r = (roughness + 1);
	float k = r*r/8;

	float num = NdotV;
	float denom = NdotV*(1-k)+k;

	return num / denom;

}
float GeometrySmith(vec3 N,vec3 V,vec3 L,float roughness)
{
	float NdotV = max(dot(N,V),0.0);
	float NdotL = max(dot(N,L),0.0);
	float ggx1 = GeometrySchlickGGX(NdotL,roughness);
	float ggx2 = GeometrySchlickGGX(NdotV,roughness);
	
	return ggx1 * ggx2;
}