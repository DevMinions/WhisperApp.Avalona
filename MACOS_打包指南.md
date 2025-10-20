# macOS 打包完整指南

## 🍎 为 macOS 创建 .app 应用包

### 方法 1：使用自动打包脚本（推荐）

#### 步骤 1：准备环境

确保您在 macOS 系统上，并已安装 .NET 9.0 SDK:

```bash
# 检查 .NET 版本
dotnet --version
```

#### 步骤 2：运行打包脚本

```bash
# 进入项目目录
cd WhisperApp.Avalona

# 给脚本添加执行权限
chmod +x build-macos.sh

# 运行脚本
./build-macos.sh
```

#### 步骤 3：选择架构

脚本会询问您要打包的架构：

```
请选择目标架构:
1) Apple Silicon (M1/M2/M3) - arm64
2) Intel - x64
3) 通用包 (两者都打包)
```

**建议：**
- 如果您的 Mac 是 M1/M2/M3，选择 1
- 如果是 Intel Mac，选择 2
- 如果要分发给其他人，选择 3

#### 步骤 4：等待完成

脚本会自动：
1. 编译应用
2. 创建 .app 包结构
3. 配置 Info.plist
4. 设置权限
5. 移除隔离属性

#### 步骤 5：运行应用

```bash
# 双击生成的 .app 文件
# 或使用命令行
open WhisperApp-AppleSilicon.app
# 或
open WhisperApp-Intel.app
```

---

### 方法 2：手动打包

如果您想手动控制每一步：

#### 第 1 步：发布应用

```bash
# 对于 Apple Silicon (M1/M2/M3)
dotnet publish -c Release -r osx-arm64 --self-contained

# 或对于 Intel Mac
dotnet publish -c Release -r osx-x64 --self-contained
```

#### 第 2 步：创建 .app 包结构

```bash
# 创建目录
mkdir -p WhisperApp.app/Contents/MacOS
mkdir -p WhisperApp.app/Contents/Resources

# 复制文件（根据您选择的架构调整路径）
cp -r bin/Release/net9.0/osx-arm64/publish/* WhisperApp.app/Contents/MacOS/
```

#### 第 3 步：创建 Info.plist

```bash
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
    <string>1.0.0</string>
    <key>CFBundleShortVersionString</key>
    <string>1.0</string>
    <key>CFBundlePackageType</key>
    <string>APPL</string>
    <key>CFBundleExecutable</key>
    <string>WhisperApp.Avalonia</string>
    <key>LSMinimumSystemVersion</key>
    <string>10.15</string>
    <key>NSHighResolutionCapable</key>
    <true/>
    <key>NSPrincipalClass</key>
    <string>NSApplication</string>
</dict>
</plist>
EOF
```

#### 第 4 步：设置权限

```bash
# 设置可执行权限
chmod +x WhisperApp.app/Contents/MacOS/WhisperApp.Avalonia

# 移除隔离属性（允许运行未签名的应用）
xattr -cr WhisperApp.app
```

#### 第 5 步：测试运行

```bash
# 双击 WhisperApp.app
# 或
open WhisperApp.app
```

---

## 🔐 处理 macOS 安全提示

### 问题：无法打开应用

如果看到："无法打开 'WhisperApp'，因为 Apple 无法验证其是否包含恶意软件"

#### 解决方案 1：使用命令行

```bash
xattr -cr WhisperApp.app
```

#### 解决方案 2：系统偏好设置

1. 尝试打开应用（会失败）
2. 打开 **系统偏好设置** → **安全性与隐私**
3. 在"通用"标签页底部，点击"仍要打开"
4. 再次打开应用

#### 解决方案 3：右键打开

1. 右键点击 `WhisperApp.app`
2. 选择"打开"
3. 在弹出的对话框中点击"打开"

---

## 📦 创建 DMG 安装包（可选）

如果您想创建专业的 .dmg 安装文件：

### 使用 create-dmg（推荐）

```bash
# 安装 create-dmg（如果尚未安装）
brew install create-dmg

# 创建 DMG
create-dmg \
  --volname "WhisperApp" \
  --volicon "icon.icns" \
  --window-pos 200 120 \
  --window-size 800 400 \
  --icon-size 100 \
  --icon "WhisperApp.app" 200 190 \
  --hide-extension "WhisperApp.app" \
  --app-drop-link 600 185 \
  "WhisperApp-Installer.dmg" \
  "WhisperApp.app"
```

### 手动创建 DMG

```bash
# 1. 创建一个临时目录
mkdir WhisperApp-DMG
cp -r WhisperApp.app WhisperApp-DMG/

# 2. 创建 DMG
hdiutil create -volname "WhisperApp" -srcfolder WhisperApp-DMG -ov -format UDZO WhisperApp.dmg

# 3. 清理
rm -rf WhisperApp-DMG
```

---

## 🎨 添加应用图标（可选）

如果您想为应用添加自定义图标：

### 第 1 步：准备图标

1. 准备一个 1024x1024 的 PNG 图像
2. 使用在线工具或命令行转换为 .icns 格式

### 第 2 步：添加图标

```bash
# 将图标复制到 Resources 目录
cp icon.icns WhisperApp.app/Contents/Resources/

# 更新 Info.plist
# 在 <dict> 中添加：
# <key>CFBundleIconFile</key>
# <string>icon</string>
```

---

## 🚀 分发应用

### 选项 1：直接分发 .app

1. 压缩 .app 文件：
   ```bash
   zip -r WhisperApp.zip WhisperApp.app
   ```

2. 分发 .zip 文件

3. 用户解压后可以直接使用

### 选项 2：分发 DMG

1. 创建 DMG（见上文）
2. 分发 .dmg 文件
3. 用户双击 DMG，拖拽应用到"应用程序"文件夹

### 选项 3：代码签名（专业）

如果您有 Apple Developer 账户：

```bash
# 签名应用
codesign --force --deep --sign "Developer ID Application: Your Name" WhisperApp.app

# 公证应用（需要 Apple Developer 账户）
xcrun notarytool submit WhisperApp.zip --apple-id your@email.com --password your-app-specific-password --team-id TEAMID
```

---

## 📝 常见问题

### Q: 应用启动很慢？
**A:** 第一次启动可能需要几秒钟，这是正常的。后续启动会更快。

### Q: 无法在 Dock 中显示图标？
**A:** 确保您的 Info.plist 配置正确，并且 LSUIElement 没有设置为 true。

### Q: 应用在 M1 Mac 上运行但很慢？
**A:** 确保您打包的是 `osx-arm64` 版本，而不是 `osx-x64`（Intel）版本。

### Q: 文件对话框无法打开？
**A:** 检查是否给予了文件访问权限。可能需要在系统偏好设置中允许。

---

## ✅ 打包检查清单

在分发前检查：

- [ ] 应用可以正常启动
- [ ] 所有功能正常工作
- [ ] 文件对话框可以打开
- [ ] 可以选择和转录音频文件
- [ ] 可以导出结果
- [ ] 应用图标显示正确（如果添加了）
- [ ] Info.plist 配置正确
- [ ] 移除了所有调试符号
- [ ] 压缩包大小合理（~80-100 MB）

---

## 🎯 最终文件

成功打包后，您应该有：

```
WhisperApp-AppleSilicon.app  (用于 M1/M2/M3 Mac)
WhisperApp-Intel.app          (用于 Intel Mac)
WhisperApp.dmg                (安装程序，可选)
```

---

**🎉 完成！现在您可以在 macOS 上运行 WhisperApp 了！**

如有问题，请参考：
- [快速开始.md](快速开始.md)
- [README_CN.md](README_CN.md)
- [项目说明.txt](项目说明.txt)

