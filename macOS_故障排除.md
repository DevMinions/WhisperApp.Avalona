# 🍎 macOS 故障排除指南

## ❌ 常见错误

### 错误 1：应用已损坏，无法打开

**错误信息**：
```
"WhisperApp-AppleSilicon" is damaged and can't be opened. 
You should move it to the Trash.
```

**原因**：macOS 的 Gatekeeper 安全机制阻止未签名应用运行

## ✅ 解决方案

### 方法 1：移除隔离属性（推荐）

在终端中运行：

```bash
# 进入应用所在目录
cd ~/Downloads  # 或者你解压应用的位置

# 移除隔离属性
xattr -cr WhisperApp-AppleSilicon.app

# 或者如果是压缩包
xattr -cr WhisperApp-macOS-AppleSilicon.zip
```

**然后重新尝试打开应用**

### 方法 2：右键打开

1. **不要直接双击** `.app` 文件
2. **右键点击** `WhisperApp-AppleSilicon.app`
3. 选择 **"打开"** (Open)
4. 在弹出的对话框中点击 **"打开"**

### 方法 3：系统偏好设置

1. 尝试打开应用（会失败）
2. 打开 **系统偏好设置** → **安全性与隐私**
3. 在"通用"标签页底部，会看到：
   ```
   "WhisperApp-AppleSilicon" 被阻止，因为它不是来自已识别的开发者。
   [仍要打开]
   ```
4. 点击 **"仍要打开"**

### 方法 4：临时禁用 Gatekeeper

```bash
# 临时禁用 Gatekeeper（不推荐，但有效）
sudo spctl --master-disable

# 运行应用后，记得重新启用
sudo spctl --master-enable
```

## 🔍 详细步骤

### 步骤 1：检查文件状态

```bash
# 查看文件的隔离属性
xattr -l WhisperApp-AppleSilicon.app

# 如果看到 com.apple.quarantine，说明被隔离了
```

### 步骤 2：移除隔离属性

```bash
# 移除所有扩展属性
xattr -cr WhisperApp-AppleSilicon.app

# 验证是否移除成功
xattr -l WhisperApp-AppleSilicon.app
# 应该没有输出
```

### 步骤 3：重新运行

现在应该可以正常打开应用了。

## 🛠️ 预防措施

### 在构建时自动移除隔离属性

我们的 GitHub Actions 已经配置了自动移除隔离属性：

```yaml
# 移除隔离属性（允许运行未签名应用）
xattr -cr "$APP_NAME" || true

# 再次移除 ZIP 文件的隔离属性
xattr -cr "WhisperApp-macOS-${{ matrix.arch.name }}.zip" || true
```

### 本地构建时

如果你在本地构建，记得运行：

```bash
# 构建完成后
xattr -cr WhisperApp.app
```

## 📋 检查清单

遇到问题时检查：

- [ ] 是否直接双击了 `.app` 文件？
- [ ] 是否尝试了右键打开？
- [ ] 是否运行了 `xattr -cr` 命令？
- [ ] 是否检查了系统偏好设置？
- [ ] 文件是否完整下载？

## 🔐 安全说明

### 为什么会出现这个问题？

1. **Gatekeeper 保护**：macOS 默认阻止未签名应用
2. **隔离属性**：下载的文件被标记为"隔离"状态
3. **安全机制**：防止恶意软件运行

### 这样做安全吗？

✅ **是安全的**，因为：
- 这是你自己构建的应用
- 源代码是公开的
- 只是移除了隔离标记，没有修改应用本身

## 🎯 长期解决方案

### 代码签名（专业方案）

如果你有 Apple Developer 账户，可以签名应用：

```bash
# 签名应用
codesign --deep --force --sign "Developer ID Application: Your Name" WhisperApp.app

# 公证应用（需要 Apple Developer 账户）
xcrun notarytool submit WhisperApp.zip --apple-id your@email.com --password your-app-specific-password --team-id TEAMID
```

### 使用 Homebrew 分发

```bash
# 创建 Homebrew formula
brew tap your-username/whisperapp
brew install whisperapp
```

## 📞 仍然有问题？

### 检查应用完整性

```bash
# 检查应用结构
ls -la WhisperApp-AppleSilicon.app/Contents/MacOS/

# 检查可执行文件
file WhisperApp-AppleSilicon.app/Contents/MacOS/WhisperApp.Avalonia

# 检查权限
ls -la WhisperApp-AppleSilicon.app/Contents/MacOS/WhisperApp.Avalonia
```

### 重新下载

如果问题持续存在：

1. 删除当前文件
2. 重新从 GitHub Releases 下载
3. 按照上述步骤操作

### 联系支持

如果问题仍然存在，请：

1. 提供 macOS 版本信息：`sw_vers`
2. 提供错误截图
3. 提供终端输出：`xattr -l WhisperApp-AppleSilicon.app`

## 💡 提示

- **首次运行**：macOS 可能需要几秒钟来验证应用
- **权限问题**：确保应用有执行权限
- **网络问题**：某些安全功能需要网络连接

---

**🎉 按照上述步骤，你的 macOS 应用应该可以正常运行了！**

有任何问题随时联系！😊
