# LearnOpenGL 综合渲染 Demo

基于 GLFW + GLAD + GLM + Assimp 实现的 Windows OpenGL 渲染器，参考 LearnOpenGL 教程完成。

## 📋 功能特性

### 核心渲染功能
- **多光源系统**
  - 方向光（日光）
  - 4 个点光源（带衰减）
  - 聚光灯（手电筒效果）
- **材质系统**
  - 漫反射贴图（Diffuse Map）
  - 镜面光贴图（Specular Map）
  - 反光度（Shininess）控制
- **摄像机系统**
  - 自由漫游（WASD 移动 + 鼠标视角）
  - 透视投影
  - 缩放控制
- **渲染模式**
  - 实体渲染
  - 线框模式切换

### 技术特性
- OpenGL 3.3 Core Profile
- 深度测试
- 多立方体渲染（10 个不同位置的立方体）
- 灯光可视化（白色立方体表示点光源位置）

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
| **GLM** | 0.9.9+ | 数学库（矩阵、向量） |
| **Assimp** | 5.x | 3D 模型导入 |
| **stb_image** | - | 图像加载 |

## 🚀 编译运行步骤

### 方法一：使用 Visual Studio（推荐）

1. 克隆仓库
   ```bash
   git clone https://github.com/你的用户名/learnOpenGL.git
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
| `ESC` | 退出程序 |

## 📁 项目目录结构

```
learnOpenGL/
│
├── assets/                  # 资源文件
│   ├── textures/            # 纹理图片
│   │   ├── container2.png
│   │   └── container2_specular.png
│   ├── shaders/             # 着色器文件
│   │   ├── 1.colors.vs     # 顶点着色器
│   │   ├── 1.colors.fs     # 片段着色器
│   │   └── 1.colors_cube.vs/fs  # 灯光立方体着色器
│   └── models/              # 3D 模型
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

## 💡 代码架构

### 主程序流程
```
1. 初始化 GLFW + GLAD
2. 配置窗口和输入回调
3. 加载着色器和纹理
4. 配置顶点数据（VAO/VBO）
5. 渲染循环：
   - 处理输入
   - 清屏
   - 设置光照参数
   - 渲染立方体
   - 渲染灯光指示器
   - 交换缓冲区
6. 清理资源
```

### 关键类说明
- **Camera**：摄像机控制（FPS 风格）
- **Shader**：着色器程序封装
- **Mesh**：网格数据管理
- **Model**：3D 模型加载和渲染

## 🐛 常见问题

### 1. 编译错误：找不到 .lib 文件
**解决方案**：检查 `lib/` 目录是否包含所需的库文件，确保编译平台选择的是 **x64**。

### 2. 运行后黑屏
**解决方案**：
- 检查 `assets/` 目录是否在正确位置
- 确认工作目录设置为项目根目录

### 3. 纹理加载失败
**解决方案**：确保 `assets/textures/` 目录下有 `container2.png` 和 `container2_specular.png` 文件。

### 4. 中文输入法干扰
**说明**：程序启动时自动切换为英文输入法，避免摄像机控制被输入法捕获。

## 📚 参考资源

- [LearnOpenGL 中文教程](https://learnopengl-cn.github.io/)
- [LearnOpenGL 英文教程](https://learnopengl.com/)
- [GLFW 官方文档](https://www.glfw.org/docs/latest/)
- [GLM 数学库](https://glm.g-truc.net/)

## 📝 更新日志

### v1.0 (2026-07-04)
- 初始版本
- 实现基础光照（Phong 光照模型）
- 支持多光源系统
- 实现摄像机自由漫游
- 添加纹理贴图支持

## 📄 许可证

本项目仅供学习使用，遵循 LearnOpenGL 教程的开源精神。

---

**作者**：lou  
**最后更新**：2026-07-04
