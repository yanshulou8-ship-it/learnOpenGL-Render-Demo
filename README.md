# LearnOpenGL2 — PBR 分支

基于 OpenGL 3.3 Core Profile 的 **PBR（Physically Based Rendering）物理渲染** 学习项目。

> 本分支专注于 Cook-Torrance 微表面模型 + 金属-粗糙度工作流的实现与调试。

## 当前功能

- **PBR 直接光照**：基于 Cook-Torrance BRDF 的逐像素光照
  - D（法线分布函数）：GGX/Trowbridge-Reitz
  - G（几何函数）：Schlick-GGX
  - F（菲涅尔项）：Schlick 近似
- **7×7 材质球网格**：展示 metallic（行）× roughness（列）的材质变化
- **4 点光源**：实时渲染光源位置标记
- **HDR 色调映射 + Gamma 校正**：Reinhard 算子
- **程序化球体生成**：64×64 分段，Triangle Strip 渲染
- **自由摄像机**：WASD 移动 + 鼠标视角

## 编译环境

| 依赖 | 版本 |
|------|------|
| Visual Studio | 2019 / 2022 |
| Windows SDK | 10+ |
| 平台 | x64 |

## 依赖库

| 库 | 用途 |
|----|------|
| [GLFW](https://www.glfw.org/) | 窗口管理 + 输入 |
| [GLAD](https://glad.dav1d.de/) | OpenGL 函数加载 |
| [GLM](https://github.com/g-truc/glm) | 数学库 |
| [stb_image](https://github.com/nothings/stb) | 图像加载 |

## 编译运行

```bash
# 1. 切换到 PBR 分支
git checkout PBR_test

# 2. 打开 Visual Studio 解决方案
learnOpenGL.sln

# 3. 编译配置选择 x64-Debug，生成并运行（Ctrl+F5）
```

## 操作按键

| 按键 | 功能 |
|------|------|
| `W` `A` `S` `D` | 相机前后左右移动 |
| `Q` `E` | 相机上下移动 |
| 鼠标移动 | 视角旋转 |
| 鼠标滚轮 | 缩放视野（FOV） |
| `ESC` | 退出 |

## PBR 渲染管线

### Cook-Torrance BRDF

```glsl
f_r = k_d * c/π + k_s * D * F * G / (4 * (n·v) * (n·l))
```

| 项 | 实现 | 公式 |
|----|------|------|
| **NDF (D)** | GGX / Trowbridge-Reitz | `α² / (π * ((N·H)² * (α²-1) + 1)²)` |
| **Geometry (G)** | Schlick-GGX | `(N·v) / ((N·v) * (1-k) + k)` ，`k = (rough+1)²/8` |
| **Fresnel (F)** | Schlick 近似 | `F₀ + (1-F₀) * (1 - H·V)^5` |

### 衰减与色调映射

```
辐射度 = lightColor / distance²             （平方反比衰减）
最终颜色 = ambient + Σ(BRDF × 辐射度 × NdotL)
最终输出 = pow(color / (color+1), 1/2.2)     （Reinhard + Gamma）
```

## 项目结构

```
learnOpenGL2/
├── assets/shaders/
│   ├── PBR.vs                         # PBR 顶点着色器
│   └── PBR.fs                         # PBR 片段着色器
├── include/
│   ├── camera.h                       # 摄像机类
│   ├── shader.h                       # 着色器工具类
│   ├── model.h / mesh.h               # 模型加载（本分支未使用）
│   └── stb_image.h                    # 纹理加载
├── src/
│   ├── learnOpenGL.cpp                # ★ 主程序（PBR 渲染逻辑）
│   ├── glad.c                         # OpenGL 加载器
│   └── stb_image.cpp                  # 图像加载实现
└── lib/                               # 预编译库
```

## 核心代码说明

### `learnOpenGL.cpp`

```cpp
// 7×7 材质球网格
for (row = 0; row < 7; ++row) {
    shader.setFloat("metallic", row/7);       // 0.0 → 1.0
    for (col = 0; col < 7; ++col) {
        shader.setFloat("roughness", col/7);  // 0.05 → 1.0
        model = translate(position);          // 排列在 XY 平面
        renderSphere();                       // 调用程序球体
    }
}

// 4 个点光源（也渲染为小球体）
for (i = 0; i < 4; ++i) {
    shader.setVec3("lightPositions["+i+"]", lightPositions[i]);
    shader.setVec3("lightColors["+i+"]", lightColors[i]);
    renderSphere();  // 光源标记
}
```

### `renderSphere()`

- 懒初始化模式：首次调用生成 VAO，后续直接复用
- 球面参数方程：`(cos(θ)sin(φ), cos(φ), sin(θ)sin(φ))`
- 64×64 分段，Triangle Strip + 蛇形索引遍历
- 每顶点：position(3) + normal(3) + texcoord(2)

## TODO

- [ ] IBL 实现（等距圆柱投影 → 立方体贴图 → 辐照度贴图 → 预过滤反射贴图）
- [ ] PBR 纹理贴图管线（albedo、normal、metallic、roughness、AO）
- [ ] 材质球数量参数化（替换硬编码的 7×7）
- [ ] 点光源动态旋转效果
- [ ] UI 面板（ImGui）调节材质参数

## 参考

- [LearnOpenGL PBR - Theory](https://learnopengl.com/PBR/Theory)
- [LearnOpenGL PBR - Lighting](https://learnopengl.com/PBR/Lighting)

## 许可证

MIT License
