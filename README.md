# LearnOpenGL 综合渲染 Demo
基于 GLFW + GLAD + GLM + Assimp 实现Windows OpenGL渲染器
## 功能特性
- 多模型加载（背包、人物、地面平面）
- 点光源立方阴影
- Bloom泛光后处理
- MSAA多重采样抗锯齿
- 天空盒环境贴图
- 相机自由漫游、手电筒聚光灯、线框模式切换

## 编译环境
Visual Studio 2019 / 2022 Windows x64

## 编译运行步骤
1. git clone https://github.com/你的用户名/learnOpenGL-Renderer-Demo.git
2. 打开 learnOpenGL.sln
3. 编译配置选择 x64-Debug
4. 直接生成并启动调试

## 操作按键
- WASD / EQ：相机移动上下
- 鼠标：视角旋转
- 滚轮：缩放视野
- C：切换线框/实体模式
- F：开关手电筒聚光灯
- ESC：退出程序

## 项目目录结构
