# LearnOpenGL 综合渲染 Demo

基于 GLFW + GLAD + GLM + Assimp 实现的 Windows OpenGL 渲染器，参考 LearnOpenGL 教程完成。

## 🎮 场景展示

本渲染器展示了一个综合场景，包含：
- **背包模型**：使用 PBR 材质的高质量 3D 模型
- **砖块平面**：应用了视差贴图技术的地面
- **天空盒**：动态立方体贴图环境

## 📋 功能特性

### 🎨 核心渲染功能

#### 模型与纹理
- **3D 模型加载**：基于 Assimp 的多格式模型导入
- **PBR 材质系统**：
  - 漫反射贴图（Albedo/Diffuse）
  - 法线贴图（Normal Map）
  - 高光贴图（Specular Map）
  - 粗糙度贴图（Roughness Map）
  - 环境光遮蔽贴图（AO Map）

#### 高级纹理技术
- **视差贴图（Parallax Mapping）**：砖块平面的深度错觉效果
- **法线贴图（Normal Mapping）**：表面细节增强
- **立方体贴图（Cubemap）**：天空盒环境

#### 光照系统
- **多光源支持**：
  - 方向光（Directional Light）- 模拟日光
  - 点光源（Point Light）- 位置光源，支持衰减
  - 聚光灯（Spot Light）- 手电筒效果
- **阴影技术**：
  - 方向光阴影映射（Directional Shadow Mapping）
  - 点光源阴影（Point Light Shadows / Omnidirectional Shadows）

#### 延迟渲染管线
- **Geometry Pass**：将几何数据写入 G-Buffer
  - 位置缓冲（Position Buffer）
  - 法线缓冲（Normal Buffer）
  - 反照率+高光缓冲（Albedo+Specular Buffer）
- **Lighting Pass**：从 G-Buffer 计算光照
- **优势**：支持大量光源的高效渲染

#### 屏幕空间效果
- **SSAO（屏幕空间环境光遮蔽）**：
  - 增强角落和缝隙的真实感
  - 可配置的采样半径和噪声纹理
  - SSAO 模糊处理，减少噪声
- **泛光效果（Bloom）**：高亮区域发光效果
- **HDR 色调映射**：高动态范围渲染

#### 后处理
- **高斯模糊（Gaussian Blur）**：用于阴影柔和化或 Bloom 效果
- **屏幕空间后处理**：全屏特效

#### 其他特效
- **反射效果（Reflection）**：基于环境贴图的镜面反射
- **几何着色器应用**：
  - 法线可视化（Normal Visualization）
  - 阴影立方体贴图生成

### 🛠️ 技术特性

- **OpenGL 版本**：3.3 Core Profile
- **渲染技术**：
  - 延迟渲染（Deferred Rendering）
  - 前向渲染（Forward Rendering）
  - 多渲染目标（MRT）
- **深度测试**：精确的遮挡关系
- **面剔除**：提升渲染性能
- **摄像机系统**：
  - 自由漫游（WASD 移动 + 鼠标视角）
  - 透视投影
  - 缩放控制（FOV）
- **渲染模式切换**：实体/线框模式

## 🛠️ 编译环境

### 必需工具
- **Visual Studio 2019 或 2022**（推荐 2022）
- **Windows 10/11 x64**

### 项目配置
- C++ 标准：C++20
- 平台：x64
- 配置：Debug / Release

## 📦 依赖库

| 库名 | 版本 | 用途 |
|------|------|------|
| **GLFW** | 3.x | 窗口创建、输入处理 |
| **GLAD** | - | OpenGL 函数加载 |
| **GLM** | 0.9.9+ | 数学库（矩阵、向量、变换） |
| **Assimp** | 5.x | 3D 模型导入（支持 OBJ, FBX, GLTF 等） |
| **stb_image** | - | 图像加载（支持 PNG, JPG, TGA 等） |

## 🎓 使用的 OpenGL 技术栈

### 基础技术
- ✅ 顶点缓冲对象（VBO）
- ✅ 顶点数组对象（VAO）
- ✅ 元素缓冲对象（EBO）
- ✅ 着色器程序（Shader Program）
- ✅ 纹理贴图（Texture Mapping）
- ✅ 变换矩阵（模型、视图、投影）

### 光照与材质
- ✅ Phong 光照模型
- ✅ Blinn-Phong 光照模型
- ✅ 多光源系统（方向光 + 点光源 + 聚光灯）
- ✅ 光照衰减（Constant + Linear + Quadratic）
- ✅ 聚光灯平滑边缘（Smooth/Soft Edges）

### 高级纹理技术
- ✅ 漫反射贴图（Diffuse Map）
- ✅ 镜面光贴图（Specular Map）
- ✅ 法线贴图（Normal Map）
- ✅ 视差贴图（Parallax Mapping）
- ✅ 立方体贴图（Cubemap / Skybox）
- ✅ HDR 纹理支持

### 阴影技术
- ✅ 阴影映射（Shadow Mapping）
- ✅ 百分比渐近过滤（PCF - Percentage Closer Filtering）
- ✅ 点光源阴影（Point Light Shadows / Shadow Cubemap）
- ✅ 深度立方体贴图（Depth Cubemap）

### 高级渲染技术
- ✅ 延迟渲染（Deferred Rendering）
- ✅ 多渲染目标（MRT - Multiple Render Targets）
- ✅ G-Buffer 架构
- ✅ SSAO（屏幕空间环境光遮蔽）
- ✅ 泛光效果（Bloom）
- ✅ HDR 色调映射（Tone Mapping）
- ✅ Gamma 校正（Gamma Correction）

### 后处理效果
- ✅ 高斯模糊（Gaussian Blur）
- ✅ 核效果（Kernel Effects）
- ✅ 边缘检测（Edge Detection）
- ✅ 灰度化（Grayscale）
- ✅ 反相（Inversion）
- ✅ 锐化（Sharpen）
- ✅ 模糊（Blur）
- ✅ 浮雕（Emboss）

### 几何处理
- ✅ 面剔除（Face Culling）
- ✅ 深度测试（Depth Testing）
- ✅ 模板测试（Stencil Testing）
- ✅ 几何着色器（Geometry Shader）
- ✅ 实例化渲染（Instanced Rendering）

### 模型加载
- ✅ Assimp 模型加载
- ✅ 网格（Mesh）管理
- ✅ 多纹理材质支持
- ✅ 模型变换层级

### 摄像机与交互
- ✅ FPS 风格摄像机
- ✅ 鼠标视角控制
- ✅ 键盘移动控制
- ✅ 滚轮缩放（FOV）
- ✅ 碰撞检测（可选）

## 🚀 编译运行步骤

### 方法一：使用 Visual Studio（推荐）

1. 克隆仓库
   ```bash
   git clone https://github.com/yanshulou8-ship-it/learnOpenGL-Render-Demo.git
   ```

2. 打开项目
   - 双击 `learnOpenGL.sln` 用 Visual Studio 打开

3. 配置编译
   - 平台选择：**x64**
   - 配置选择：**Debug** 或 **Release**

4. 编译运行
   - 按 `Ctrl + F5` 运行（不调试）
   - 按 `F5` 运行并调试

### 方法二：使用命令行编译

```bash
# 使用 MSBuild 编译
msbuild learnOpenGL.sln /p:Configuration=Debug /p:Platform=x64
```

## 🎮 操作按键

### 摄像机控制
| 按键 | 功能 |
|------|------|
| `W` / `S` | 前进 / 后退 |
| `A` / `D` | 左移 / 右移 |
| `E` / `Q` | 上升 / 下降 |
| 鼠标移动 | 视角旋转 |
| 鼠标滚轮 | 缩放视野（FOV） |

### 功能切换
| 按键 | 功能 |
|------|------|
| `C` | 切换线框/实体模式 |
| `F` | 开关手电筒聚光灯 |
| `B` | 开关泛光效果（Bloom） |
| `ESC` | 退出程序 |

## 📁 项目目录结构

```
learnOpenGL/
│
├── assets/                  # 资源文件
│   ├── textures/            # 纹理图片
│   │   ├── skybox/         # 天空盒纹理（6张）
│   │   ├── container2.png
│   │   ├── container2_specular.png
│   │   └── wall.jpg
│   ├── shaders/             # 着色器文件
│   │   ├── basic.vs/fs     # 基础着色器
│   │   ├── skyBox.vs/fs    # 天空盒着色器
│   │   ├── shadow_mapping_depth.vs/fs  # 阴影映射
│   │   ├── point_shadows_depth.vs/gs/fs  # 点光源阴影
│   │   ├── g_buffer.vs/fs  # 延迟渲染 G-Buffer
│   │   ├── deferred_lighting.fs  # 延迟渲染光照
│   │   ├── ssao.vs/fs      # SSAO
│   │   ├── ssao_blur.vs/fs # SSAO 模糊
│   │   ├── blur.vs/fs      # 高斯模糊
│   │   ├── reflect.vs/fs   # 反射效果
│   │   └── drawNormals.gs  # 法线可视化
│   └── models/              # 3D 模型
│       ├── backpack/        # 背包模型（OBJ + 多张贴图）
│       ├── ling/            # 其他模型
│       ├── plane/           # 平面模型
│       └── su/             # 其他模型
│
├── include/                 # 自定义头文件
│   ├── camera.h            # 摄像机类
│   ├── shader.h            # 着色器封装类
│   ├── model.h             # 3D 模型加载类
│   ├── mesh.h              # 网格类
│   └── stb_image.h         # 图像加载库
│
├── src/                     # 源文件
│   ├── learnOpenGL.cpp     # 主程序
│   ├── glad.c              # GLAD 源码
│   └── stb_image.cpp       # stb_image 实现
│
├── includes/                # 第三方库头文件
│   ├── glad/
│   ├── GLFW/
│   ├── glm/
│   └── assimp/
│
├── lib/                     # 预编译库文件
│   ├── glfw3.lib
│   └── assimp-vc143-mtd.lib
│
├── learnOpenGL.sln         # Visual Studio 解决方案
├── learnOpenGL.vcxproj     # 项目文件
└── README.md               # 本文件
```

## 🎬 场景说明

### 场景组成
本渲染器展示了一个综合 3D 场景：

1. **背包模型（Backpack）**
   - 格式：OBJ 模型
   - 材质：PBR 材质系统
   - 贴图：
     - `diffuse.jpg` - 漫反射贴图
     - `normal.png` - 法线贴图
     - `specular.jpg` - 高光贴图
     - `roughness.jpg` - 粗糙度贴图
     - `ao.jpg` - 环境光遮蔽贴图

2. **砖块平面（Brick Plane）**
   - 应用了视差贴图技术
   - 实现深度错觉效果
   - 包含法线贴图和高度贴图

3. **天空盒（Skybox）**
   - 立方体贴图环境
   - 6 张纹理：front, back, left, right, top, bottom
   - 支持 HDR 和高亮提取（用于 Bloom 效果）

### 渲染管线流程
```
1. 几何通道（Geometry Pass）[延迟渲染]
   - 渲染场景到 G-Buffer
   - 存储位置、法线、反照率、高光
   - 应用视差贴图修正纹理坐标

2. SSAO 通道
   - 计算屏幕空间环境光遮蔽
   - 应用模糊滤波

3. 光照通道（Lighting Pass）[延迟渲染]
   - 从 G-Buffer 读取几何信息
   - 计算多光源光照
   - 应用阴影
   - 应用 SSAO 遮蔽

4. 前向渲染通道（可选）
   - 渲染透明物体
   - 渲染特效

5. 后处理通道
   - 泛光效果（Bloom）
   - HDR 色调映射
   - Gamma 校正

6. 天空盒渲染
   - 最后渲染，深度测试优化
```

## 💡 代码架构

### 主程序流程
```
1. 初始化 GLFW + GLAD
2. 配置窗口和输入回调
3. 加载着色器程序
   - 基础渲染着色器
   - 阴影映射着色器
   - 延迟渲染着色器
   - SSAO 着色器
   - 后处理着色器
   - 天空盒着色器
4. 加载模型和纹理
   - 背包模型（Assimp）
   - 砖块平面纹理
   - 天空盒纹理
5. 配置帧缓冲区
   - G-Buffer（延迟渲染）
   - SSAO FBO
   - 后处理 FBO
6. 渲染循环：
   - 处理输入
   - 清屏
   - 渲染到 G-Buffer
   - 计算 SSAO
   - 光照计算（延迟渲染）
   - 后处理（Bloom + HDR + Gamma）
   - 渲染天空盒
   - 交换缓冲区
7. 清理资源
```

### 关键类说明
- **Camera**：摄像机控制（FPS 风格，支持透视/正交投影）
- **Shader**：着色器程序封装（支持 uniform 设置、编译、链接）
- **Mesh**：网格数据管理（VAO/VBO/EBO 配置）
- **Model**：3D 模型加载和渲染（基于 Assimp）
- **stb_image 封装**：图像加载工具函数

## 🐛 常见问题

### 1. 编译错误：找不到 .lib 文件
**解决方案**：检查 `lib/` 目录是否包含所需的库文件，确保编译平台选择的是 **x64**。

### 2. 运行后黑屏
**解决方案**：
- 检查 `assets/` 目录是否在正确位置
- 确认工作目录设置为项目根目录
- 检查着色器文件是否正确加载

### 3. 纹理加载失败
**解决方案**：确保 `assets/textures/` 目录下有相应的纹理文件。背包模型需要 5 张贴图（diffuse, normal, specular, roughness, ao）。

### 4. 阴影渲染异常
**解决方案**：
- 检查阴影映射的纹理单元设置
- 确认光源矩阵计算正确
- 调整阴影偏移（Shadow Bias）减少阴影瑕疵

### 5. SSAO 效果不明显或有噪点
**解决方案**：
- 调整 SSAO 采样半径（radius）
- 增加采样点数量（samples）
- 应用 SSAO 模糊处理

### 6. 中文输入法干扰
**说明**：程序启动时自动切换为英文输入法，避免摄像机控制被输入法捕获。

## 📚 参考资源

### 教程与文档
- [LearnOpenGL 中文教程](https://learnopengl-cn.github.io/) - 本项目的核心参考
- [LearnOpenGL 英文教程](https://learnopengl.com/) - 原版教程
- [OpenGL 官方文档](https://www.opengl.org/documentation/)
- [GLSL 参考手册](https://www.khronos.org/opengl/wiki/Core_Language_(GLSL))

### 库文档
- [GLFW 官方文档](https://www.glfw.org/docs/latest/)
- [GLM 数学库](https://glm.g-truc.net/0.9.9/index.html)
- [Assimp 文档](http://assimp.sourceforge.net/lib_html/index.html)
- [stb_image 文档](https://github.com/nothings/stb)

### 技术文章
- [延迟渲染（Deferred Rendering）](https://learnopengl-cn.github.io/05%20Advanced%20Lighting/08%20Deferred%20Shading/)
- [阴影映射（Shadow Mapping）](https://learnopengl-cn.github.io/05%20Advanced%20Lighting/03%20Shadows/01%20Shadow%20Mapping/)
- [SSAO（屏幕空间环境光遮蔽）](https://learnopengl-cn.github.io/05%20Advanced%20Lighting/09%20SSAO/)
- [视差贴图（Parallax Mapping）](https://learnopengl-cn.github.io/05%20Advanced%20Lighting/05%20Parallax%20Mapping/)
- [PBR 理论](https://learnopengl-cn.github.io/07%20PBR/01%20Theory/)

## 📝 更新日志

### v2.0 (2026-07-04)
- 🎉 重构渲染管线，支持延迟渲染
- ✨ 添加背包模型渲染（PBR 材质）
- ✨ 添加视差贴图技术（砖块平面）
- ✨ 添加天空盒渲染
- ✨ 添加阴影映射（方向光 + 点光源）
- ✨ 添加 SSAO 屏幕空间环境光遮蔽
- ✨ 添加泛光效果（Bloom）
- ✨ 添加 HDR 色调映射
- ✨ 添加后处理效果（模糊、核效果等）
- 🔧 优化摄像机控制系统
- 🔧 优化资源加载和管理

### v1.0 (2026-07-04)
- 初始版本
- 实现基础光照（Phong 光照模型）
- 支持多光源系统
- 实现摄像机自由漫游
- 添加纹理贴图支持

## 📄 许可证

本项目仅供学习使用，遵循 LearnOpenGL 教程的开源精神。

### 第三方资源许可
- **背包模型**：请参考 `assets/models/backpack/source_attribution.txt`
- **天空盒纹理**：公共领域或署名许可
- **纹理图片**：来自 LearnOpenGL 教程示例

---

**作者**：lou  
**最后更新**：2026-07-04  
**项目地址**：[GitHub](https://github.com/yanshulou8-ship-it/learnOpenGL-Render-Demo)
