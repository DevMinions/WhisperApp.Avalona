# WhisperApp.Avalonia - 跨平台音频转录应用

一个基于 Avalonia UI 的跨平台音频转录桌面应用，可在 **Windows、macOS 和 Linux** 上运行。

## ✨ 特点

- 🌍 **跨平台**: Windows、macOS、Linux 全支持
- 🎨 **现代界面**: 完全中文界面，简洁美观
- 🎵 **多格式支持**: MP3、WAV、M4A、MP4 等
- 📤 **自动上传**: 自动处理文件上传
- ⏱️ **实时反馈**: 实时显示处理进度
- 💾 **导出功能**: 保存为文本文件

## 🖥️ 系统要求

### Windows
- Windows 10 或更高版本
- .NET 9.0 Runtime

### macOS
- macOS 10.15 (Catalina) 或更高版本
- .NET 9.0 Runtime
- Apple Silicon (M1/M2/M3) 或 Intel

### Linux
- Ubuntu 20.04 或更高版本 (或其他现代 Linux 发行版)
- .NET 9.0 Runtime

## 🚀 快速开始

### 运行应用

#### Windows
```bash
dotnet run
```

#### macOS / Linux
```bash
dotnet run
```

### 使用步骤

1. **获取 API 密钥**
   - 访问：https://www.assemblyai.com/app/account
   - 注册并复制 API 密钥

2. **启动应用**
   - 输入 API 密钥
   - 点击"浏览..."选择音频文件
   - 点击"开始转录"
   - 等待完成
   - 查看结果并导出

## 📦 打包应用

### 为 macOS 打包 (.app 应用包)

```bash
# 发布应用
dotnet publish -c Release -r osx-x64 --self-contained

# 或者为 Apple Silicon (M1/M2/M3) 打包
dotnet publish -c Release -r osx-arm64 --self-contained
```

打包后的文件在: `bin/Release/net9.0/osx-x64/publish/`

### 为 Linux 打包

```bash
# 发布应用
dotnet publish -c Release -r linux-x64 --self-contained
```

### 为 Windows 打包

```bash
# 发布应用
dotnet publish -c Release -r win-x64 --self-contained
```

## 🍎 在 macOS 上创建 .app 包

创建应用包结构：

```bash
# 1. 发布应用
dotnet publish -c Release -r osx-arm64 --self-contained

# 2. 创建 .app 目录结构
mkdir -p WhisperApp.app/Contents/MacOS
mkdir -p WhisperApp.app/Contents/Resources

# 3. 复制可执行文件
cp -r bin/Release/net9.0/osx-arm64/publish/* WhisperApp.app/Contents/MacOS/

# 4. 创建 Info.plist
cat > WhisperApp.app/Contents/Info.plist << 'EOF'
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>CFBundleName</key>
    <string>WhisperApp</string>
    <key>CFBundleDisplayName</key>
    <string>音频转录</string>
    <key>CFBundleIdentifier</key>
    <string>com.whisperapp.avalonia</string>
    <key>CFBundleVersion</key>
    <string>1.0</string>
    <key>CFBundlePackageType</key>
    <string>APPL</string>
    <key>CFBundleExecutable</key>
    <string>WhisperApp.Avalonia</string>
    <key>LSMinimumSystemVersion</key>
    <string>10.15</string>
</dict>
</plist>
EOF

# 5. 设置可执行权限
chmod +x WhisperApp.app/Contents/MacOS/WhisperApp.Avalonia
```

现在可以双击 `WhisperApp.app` 运行了！

## 🐧 在 Linux 上运行

```bash
# 发布
dotnet publish -c Release -r linux-x64 --self-contained

# 运行
cd bin/Release/net9.0/linux-x64/publish/
./WhisperApp.Avalonia
```

## 🏗️ 构建

```bash
# 还原依赖
dotnet restore

# 构建
dotnet build

# 发布（所有平台）
dotnet publish -c Release
```

## 📝 支持的文件格式

### 音频格式
- MP3 (.mp3)
- WAV (.wav)
- M4A (.m4a)
- FLAC (.flac)
- OGG (.ogg)
- OPUS (.opus)
- WebM (.webm)

### 视频格式
- MP4 (.mp4)
- AVI (.avi)
- MOV (.mov)
- MKV (.mkv)

## 🔧 技术栈

- **框架**: .NET 9.0
- **UI**: Avalonia UI 11.3.6
- **API**: AssemblyAI Speech-to-Text
- **语言**: C# 12

## 📚 与 WPF 版本的区别

| 特性 | WPF 版本 | Avalonia 版本 |
|------|----------|---------------|
| Windows | ✅ | ✅ |
| macOS | ❌ | ✅ |
| Linux | ❌ | ✅ |
| 界面 | Windows 原生 | 现代跨平台 |
| 功能 | 完整 | 完整 |

## 🎯 功能完整性

Avalonia 版本保留了 WPF 版本的所有功能：

- ✅ API 密钥配置
- ✅ 文件选择
- ✅ 音频转录
- ✅ 实时进度
- ✅ 结果显示
- ✅ 文件导出
- ✅ 取消操作
- ✅ 错误处理
- ✅ 中文界面

## 🆘 故障排除

### macOS: "无法打开应用，因为 Apple 无法验证其是否包含恶意软件"

```bash
# 允许运行未签名的应用
xattr -cr WhisperApp.app

# 或在系统偏好设置中允许
```

### Linux: 缺少依赖

```bash
# Ubuntu/Debian
sudo apt-get install libx11-dev libice-dev libsm-dev

# Fedora
sudo dnf install libX11-devel libICE-devel libSM-devel
```

### 所有平台: .NET 运行时

如果没有安装 .NET 9.0:
- 访问: https://dotnet.microsoft.com/download
- 下载并安装 .NET 9.0 SDK 或 Runtime

## 📄 许可证

本项目使用 AssemblyAI API，需要遵守其服务条款。

## 🎉 开始使用

```bash
# 1. 克隆或下载项目
# 2. 进入目录
cd WhisperApp.Avalona

# 3. 运行
dotnet run

# 4. 输入 API 密钥并开始转录！
```

## 🔗 相关链接

- [AssemblyAI 文档](https://www.assemblyai.com/docs)
- [Avalonia UI 文档](https://docs.avaloniaui.net/)
- [.NET 文档](https://docs.microsoft.com/zh-cn/dotnet/)

---

**跨平台 | 开源 | 现代化**

🎊 现在您可以在任何操作系统上使用音频转录应用了！

