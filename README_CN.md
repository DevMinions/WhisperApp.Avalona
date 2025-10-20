# 🎙️ WhisperApp - 音频转录应用

[![构建状态](https://github.com/your-username/WhisperApp.Avalona/actions/workflows/build-release.yml/badge.svg)](https://github.com/your-username/WhisperApp.Avalona/actions)
[![最新版本](https://img.shields.io/github/v/release/your-username/WhisperApp.Avalona)](https://github.com/your-username/WhisperApp.Avalona/releases)
[![下载次数](https://img.shields.io/github/downloads/your-username/WhisperApp.Avalona/total)](https://github.com/your-username/WhisperApp.Avalona/releases)
[![支持平台](https://img.shields.io/badge/平台-Windows%20%7C%20macOS%20%7C%20Linux-blue)](#下载)
[![.NET 版本](https://img.shields.io/badge/.NET-9.0-512BD4)](https://dotnet.microsoft.com/)

> 基于 Avalonia UI 的跨平台音频转录应用

<p align="center">
  <a href="#english">English</a> •
  <a href="#chinese">中文</a>
</p>

---

## ✨ 特性

- 🎙️ **音频文件转录** - 将音频文件转换为文本
- 🖥️ **跨平台支持** - Windows、macOS 和 Linux
- 🚀 **自动构建发布** - GitHub Actions 自动化 CI/CD
- 📦 **开箱即用** - 无需额外配置
- 🎨 **现代界面** - 基于 Avalonia UI 框架构建

## 📥 下载

前往 [Releases](https://github.com/your-username/WhisperApp.Avalona/releases) 页面下载最新版本：

| 平台 | 下载 |
|------|------|
| 🪟 Windows (x64) | [WhisperApp-Windows-x64.zip](https://github.com/your-username/WhisperApp.Avalona/releases/latest) |
| 🍎 macOS (Apple Silicon) | [WhisperApp-macOS-AppleSilicon.zip](https://github.com/your-username/WhisperApp.Avalona/releases/latest) |
| 🍎 macOS (Intel) | [WhisperApp-macOS-Intel.zip](https://github.com/your-username/WhisperApp.Avalona/releases/latest) |
| 🐧 Linux (x64) | [WhisperApp-Linux-x64.tar.gz](https://github.com/your-username/WhisperApp.Avalona/releases/latest) |

## 🚀 快速开始

### Windows
1. 下载并解压 `WhisperApp-Windows-x64.zip`
2. 双击 `WhisperApp.Avalonia.exe` 运行

### macOS
1. 下载并解压对应版本的 zip 文件
2. 双击 `.app` 文件运行
3. 如遇安全提示，右键点击选择"打开"

### Linux
```bash
tar -xzf WhisperApp-Linux-x64.tar.gz
cd WhisperApp-Linux-x64
./run.sh
```

## 🛠️ 开发

### 环境要求
- .NET 9.0 SDK
- Windows / macOS / Linux

### 本地运行
```bash
git clone https://github.com/your-username/WhisperApp.Avalona.git
cd WhisperApp.Avalona
dotnet run
```

### 本地打包

#### Windows
```cmd
build-windows.bat
```

#### macOS
```bash
chmod +x build-macos.sh
./build-macos.sh
```

#### Linux
```bash
chmod +x build-linux.sh
./build-linux.sh
```

## 📚 文档

- [快速开始](快速开始.md)
- [macOS 打包指南](MACOS_打包指南.md)
- [GitHub Actions 自动打包](GITHUB_ACTIONS_快速指南.md)
- [更新日志](CHANGELOG.md)
- [故障排除](GITHUB_ACTIONS_故障排除.md)

## 🤝 贡献

欢迎提交 Issue 和 Pull Request！

### 如何贡献
1. Fork 本仓库
2. 创建功能分支 (`git checkout -b feature/AmazingFeature`)
3. 提交更改 (`git commit -m 'Add some AmazingFeature'`)
4. 推送到分支 (`git push origin feature/AmazingFeature`)
5. 创建 Pull Request

## 📄 许可证

本项目采用 MIT 许可证 - 查看 [LICENSE](LICENSE) 文件了解详情。

## 🙏 致谢

- [Avalonia UI](https://avaloniaui.net/) - 跨平台 UI 框架
- [.NET](https://dotnet.microsoft.com/) - 开发平台
- [GitHub Actions](https://github.com/features/actions) - CI/CD 自动化

## 📊 项目状态

![GitHub last commit](https://img.shields.io/github/last-commit/your-username/WhisperApp.Avalona)
![GitHub code size](https://img.shields.io/github/languages/code-size/your-username/WhisperApp.Avalona)
![GitHub issues](https://img.shields.io/github/issues/your-username/WhisperApp.Avalona)
![GitHub pull requests](https://img.shields.io/github/issues-pr/your-username/WhisperApp.Avalona)

---

## 🔄 语言切换

### English
This README is in Chinese. For English documentation, please see [README.md](README.md).

### 中文（当前）
此 README 为中文版本。英文文档请查看 [README.md](README.md)。

---

**⭐ 如果这个项目对你有帮助，请给个星标！**

---

<div align="center">
  <sub>使用 Avalonia UI 和 .NET 构建 ❤️</sub>
</div>