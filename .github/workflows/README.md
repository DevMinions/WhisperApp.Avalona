# GitHub Actions CI/CD 使用指南

## 🎯 功能概述

这个 GitHub Actions 工作流可以自动在以下平台上构建和打包 WhisperApp：

- 🪟 **Windows** (x64)
- 🍎 **macOS** (Apple Silicon M1/M2/M3 + Intel)
- 🐧 **Linux** (x64)

## 🚀 触发方式

### 方式 1：推送 Tag（推荐）

这是最常用的发布方式，适合正式版本发布。

```bash
# 1. 确保代码已提交
git add .
git commit -m "准备发布 v1.0.0"

# 2. 创建并推送 tag
git tag v1.0.0
git push origin v1.0.0

# 或者一次性完成
git tag v1.0.0 && git push origin v1.0.0
```

**工作流会自动：**
- ✅ 在 Windows、macOS、Linux 上构建应用
- ✅ 创建所有平台的安装包
- ✅ 创建 GitHub Release (名称：WhisperApp v1.0.0)
- ✅ 自动上传所有构建产物到 Release

### 方式 2：手动触发

适合测试或特殊情况下的构建。

1. 打开你的 GitHub 仓库
2. 点击 **Actions** 标签
3. 选择左侧的 **🚀 Build and Release**
4. 点击右上角 **Run workflow**
5. 输入版本号（例如：1.0.0）
6. 点击 **Run workflow** 按钮

### 方式 3：Pull Request 时自动构建

当创建 PR 到 `main` 或 `master` 分支时：
- ✅ 自动构建所有平台
- ✅ 验证代码可以正确编译
- ❌ 不会创建 Release
- ❌ 不会发布产物（仅保留 5 天）

## 📦 构建产物

### Windows 版本
```
WhisperApp-Windows-x64.zip
├── WhisperApp.Avalonia.exe  (主程序)
├── 运行应用.bat              (启动脚本)
└── [依赖 DLL 文件]
```

### macOS 版本
```
WhisperApp-macOS-AppleSilicon.zip  (M1/M2/M3)
└── WhisperApp-AppleSilicon.app/
    └── Contents/
        ├── Info.plist
        └── MacOS/
            └── WhisperApp.Avalonia

WhisperApp-macOS-Intel.zip  (Intel)
└── WhisperApp-Intel.app/
    └── Contents/
        ├── Info.plist
        └── MacOS/
            └── WhisperApp.Avalonia
```

### Linux 版本
```
WhisperApp-Linux-x64.tar.gz
└── WhisperApp-Linux-x64/
    ├── WhisperApp.Avalonia   (主程序)
    ├── run.sh                (启动脚本)
    ├── whisperapp.desktop    (桌面快捷方式)
    └── [依赖 .so 文件]
```

## 🔍 查看构建状态

### 实时查看
1. 打开 GitHub 仓库
2. 点击 **Actions** 标签
3. 查看最新的工作流运行状态
4. 点击具体的运行可以查看详细日志

### 构建状态徽章

可以在 `README.md` 中添加徽章显示构建状态：

```markdown
![Build Status](https://github.com/你的用户名/WhisperApp.Avalona/actions/workflows/build-release.yml/badge.svg)
```

## 📥 下载构建产物

### 正式版本（Release）
1. 进入仓库的 **Releases** 页面
2. 选择对应的版本（例如：v1.0.0）
3. 在 **Assets** 区域下载对应平台的文件

### 开发版本（Artifacts）
1. 进入 **Actions** 标签
2. 选择对应的工作流运行
3. 在页面底部的 **Artifacts** 区域下载
4. 注意：Artifacts 只保留 5 天

## 🛠️ 自定义配置

### 修改 .NET 版本

编辑 `.github/workflows/build-release.yml`：

```yaml
env:
  DOTNET_VERSION: '9.0.x'  # 修改为你需要的版本
```

### 修改触发分支

```yaml
on:
  pull_request:
    branches: [ main, master, develop ]  # 添加更多分支
```

### 添加更多平台

例如添加 Windows ARM64：

```yaml
build-windows-arm:
  name: 🪟 Build Windows ARM64
  runs-on: windows-latest
  steps:
    # ... 类似的步骤 ...
    - name: 🏗️ Build Windows (ARM64)
      run: dotnet publish -c Release -r win-arm64 --self-contained
```

### 修改保留天数

```yaml
- name: 📤 Upload artifact
  uses: actions/upload-artifact@v4
  with:
    retention-days: 30  # 修改为 30 天
```

## 📝 Release 说明自定义

编辑工作流文件中的 `body` 部分：

```yaml
- name: 🎉 Create Release
  uses: softprops/action-gh-release@v1
  with:
    body: |
      ## 你的自定义内容
      
      ### 新功能
      - 功能 1
      - 功能 2
      
      ### Bug 修复
      - 修复问题 1
```

## 🔐 高级功能

### 代码签名（可选）

#### Windows 代码签名

需要配置证书：

```yaml
- name: Sign Windows executable
  run: |
    # 使用 signtool 签名
    signtool sign /f ${{ secrets.CERT_FILE }} /p ${{ secrets.CERT_PASSWORD }} WhisperApp.Avalonia.exe
```

#### macOS 代码签名

需要 Apple Developer 账户：

```yaml
- name: Sign macOS app
  run: |
    # 导入证书
    security import ${{ secrets.MAC_CERT }} -P ${{ secrets.MAC_CERT_PASSWORD }}
    
    # 签名
    codesign --deep --force --sign "Developer ID Application: Your Name" WhisperApp.app
```

### 自动版本号

从 `csproj` 文件读取版本：

```yaml
- name: Get version from csproj
  id: version
  run: |
    VERSION=$(grep -oPm1 "(?<=<Version>)[^<]+" WhisperApp.Avalonia.csproj)
    echo "version=$VERSION" >> $GITHUB_OUTPUT
```

## 🐛 常见问题

### Q: 构建失败怎么办？

**A:** 
1. 检查 Actions 日志中的错误信息
2. 确保本地可以正常构建：`dotnet build`
3. 检查 .NET 版本是否匹配
4. 确保所有依赖都已正确配置

### Q: macOS 构建很慢？

**A:** macOS runners 通常比较慢，这是正常的。可以考虑：
- 使用缓存加速依赖安装
- 只在正式发布时构建 macOS 版本

### Q: 如何只构建特定平台？

**A:** 可以注释掉不需要的 job：

```yaml
# build-linux:  # 注释掉不需要的
#   name: 🐧 Build Linux
#   ...
```

### Q: Release 创建失败？

**A:** 检查：
1. Tag 格式是否正确（必须是 `v1.0.0` 格式）
2. 是否有权限创建 Release
3. `GITHUB_TOKEN` 是否有效（一般自动提供）

### Q: 如何在发布前测试工作流？

**A:** 使用 Pull Request 方式：
1. 创建新分支
2. 提交修改
3. 创建 PR 到 main
4. 工作流会自动运行但不创建 Release

## 💡 最佳实践

### 1. 版本号规范

使用 [语义化版本](https://semver.org/lang/zh-CN/)：

```
v1.0.0   - 重大更新
v1.1.0   - 功能更新
v1.1.1   - Bug 修复
```

### 2. 发布前检查

```bash
# 1. 本地测试所有平台
dotnet publish -c Release -r win-x64
dotnet publish -c Release -r osx-arm64
dotnet publish -c Release -r linux-x64

# 2. 运行测试（如果有）
dotnet test

# 3. 更新版本号和 CHANGELOG
# 4. 创建 tag 并推送
```

### 3. CHANGELOG 管理

创建 `CHANGELOG.md` 记录每个版本的更新：

```markdown
## [1.0.0] - 2024-01-20
### 新增
- 功能 A
- 功能 B

### 修复
- Bug X
- Bug Y
```

## 📚 参考资源

- [GitHub Actions 文档](https://docs.github.com/actions)
- [.NET 发布指南](https://docs.microsoft.com/dotnet/core/deploying/)
- [Avalonia 部署文档](https://docs.avaloniaui.net/docs/deployment/)

---

**🎉 现在你可以轻松地在所有平台上自动构建和发布你的应用了！**

