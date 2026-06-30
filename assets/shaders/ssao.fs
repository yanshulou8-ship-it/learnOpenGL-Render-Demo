#version 330 core
out float FragColor;
in vec2 TexCoords;

uniform sampler2D gPosition;
uniform sampler2D gNormal;
uniform sampler2D texNoise;

uniform vec3 samples[64];
uniform mat4 projection;

// 屏幕分辨率 800x600 除以噪声纹理大小 4x4，用于在全屏平铺噪声
const vec2 noiseScale = vec2(800.0/4.0, 600.0/4.0); 

float radius = 0.5;  // 采样半球半径
float bias = 0.025;  // 深度偏置，防止粉斑瑕疵

void main() {
    vec3 fragPos = texture(gPosition, TexCoords).xyz;
    vec3 normal = texture(gNormal, TexCoords).rgb;
    vec3 randomVec = texture(texNoise, TexCoords * noiseScale).xyz;

    // 创建 TBN 矩阵：将切线空间的采样点旋转到 View 空间
    vec3 tangent = normalize(randomVec - normal * dot(randomVec, normal));
    vec3 bitangent = cross(normal, tangent);
    mat3 TBN = mat3(tangent, bitangent, normal);

    float occlusion = 0.0;
    for(int i = 0; i < 64; ++i) {
        // 获取 View 空间下的采样点坐标
        vec3 samplePos = TBN * samples[i]; 
        samplePos = fragPos + samplePos * radius; 
        
        // 将采样点投影到屏幕坐标，转换为 UV 纹理坐标
        vec4 offset = vec4(samplePos, 1.0);
        offset = projection * offset;    
        offset.xyz /= offset.w;               
        offset.xyz = offset.xyz * 0.5 + 0.5; 
        
        // 获取采样点所在位置的实际几何体深度
        float sampleDepth = texture(gPosition, offset.xy).z; 
        
        // 范围检查：防止距离太远的背景物体错误地遮蔽前景
        float rangeCheck = smoothstep(0.0, 1.0, radius / abs(fragPos.z - sampleDepth));
        
        // 实际深度对比
        occlusion += (sampleDepth >= samplePos.z + bias ? 1.0 : 0.0) * rangeCheck;           
    }
    // 输出遮蔽结果 (0.0 为完全遮蔽黑死角，1.0 为无遮蔽)
    FragColor = 1.0 - (occlusion / 64.0);
}