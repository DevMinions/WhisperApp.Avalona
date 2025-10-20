# GitHub Actions 自动打包快速指南

## 🎯 一分钟快速上手

### 步骤 1：推送代码到 GitHub

```bash
# 如果还没有 Git 仓库，先初始化
git init
git add .
git commit -m "Initial commit"

# 在 GitHub 上创建仓库后，添加远程仓库
git remote add origin https://github.com/你的用户名/WhisperApp.Avalona.git
git push -u origin main
```

### 步骤 2：创建并推送 Tag 触发构建

```bash
# 创建版本 tag
git tag v1.0.0

# 推送 tag（这会自动触发构建）
git push origin v1.0.0
```

### 步骤 3：等待构建完成

1. 打开你的 GitHub 仓库页面
2. 点击顶部的 **Actions** 标签
3. 看到绿色的 ✓ 表示构建成功
4. 大约需要 15-20 分钟（取决于 GitHub 服务器）

### 步骤 4：下载发布包

1. 点击顶部的 **Releases** 标签
2. 找到 `v1.0.0` 版本
3. 在 **Assets** 下载对应平台的文件：
   - Windows: `WhisperApp-Windows-x64.zip`
   - macOS (M1/M2/M3): `WhisperApp-macOS-AppleSilicon.zip`
   - macOS (Intel): `WhisperApp-macOS-Intel.zip`
   - Linux: `WhisperApp-Linux-x64.tar.gz`

## 🎉 完成！

现在你有了所有平台的应用包，无需在每个平台上手动构建！

---

## 📋 三种触发方式对比

| 方式 | 命令 | 使用场景 | 是否创建 Release |
|------|------|----------|------------------|
| **推送 Tag** | `git push origin v1.0.0` | ✅ 正式发布 | ✅ 是 |
| **手动触发** | 在 GitHub Actions 页面操作 | 测试构建 | ✅ 是 |
| **Pull Request** | 创建 PR | 自动测试 | ❌ 否 |

---

## 🔄 日常开发流程

### 场景 1：日常开发（不发布）

```bash
# 正常提交代码
git add .
git commit -m "添加新功能"
git push origin main
```

**结果**：代码被推送，但不触发构建和发布。

### 场景 2：测试构建（不正式发布）

```bash
# 创建功能分支
git checkout -b feature/new-feature
git add .
git commit -m "新功能"
git push origin feature/new-feature

# 创建 Pull Request
# 到 GitHub 上创建 PR
```

**结果**：自动构建所有平台，验证代码可以编译，但不创建 Release。

### 场景 3：正式发布新版本

```bash
# 1. 确保代码已提交
git add .
git commit -m "准备发布 v1.1.0"
git push origin main

# 2. 创建并推送 tag
git tag v1.1.0
git push origin v1.1.0
```

**结果**：自动构建并创建 Release，所有平台的包都会上传。

---

## 📸 GitHub Actions 界面预览

### Actions 页面
```
┌─────────────────────────────────────────────────┐
│ Actions                                         │
├─────────────────────────────────────────────────┤
│ 🚀 Build and Release                            │
│   └─ v1.0.0 tag push             ✓ 15m ago     │
│       ├─ 🪟 Build Windows        ✓ 5m          │
│       ├─ 🍎 Build macOS (Intel)  ✓ 8m          │
│       ├─ 🍎 Build macOS (ARM64)  ✓ 8m          │
│       ├─ 🐧 Build Linux          ✓ 6m          │
│       └─ 📦 Create Release       ✓ 2m          │
└─────────────────────────────────────────────────┘
```

### Releases 页面
```
┌─────────────────────────────────────────────────┐
│ Releases                                        │
├─────────────────────────────────────────────────┤
│ v1.0.0  Latest                                  │
│ WhisperApp v1.0.0                               │
│                                                 │
│ Assets                                          │
│ 📦 WhisperApp-Windows-x64.zip        85 MB     │
│ 📦 WhisperApp-macOS-AppleSilicon.zip 92 MB     │
│ 📦 WhisperApp-macOS-Intel.zip        94 MB     │
│ 📦 WhisperApp-Linux-x64.tar.gz       88 MB     │
└─────────────────────────────────────────────────┘
```

---

## 🚨 常见错误处理

### 错误 1：推送 tag 后没有触发构建

**原因**：tag 格式不正确

```bash
# ❌ 错误
git tag 1.0.0          # 缺少 'v' 前缀

# ✅ 正确
git tag v1.0.0
```

### 错误 2：构建失败

**查看错误日志**：
1. 进入 Actions 页面
2. 点击失败的工作流
3. 展开失败的步骤查看详细错误
4. 根据错误信息修复代码

**常见原因**：
- 编译错误（语法错误）
- 缺少依赖包
- .NET 版本不匹配

### 错误 3：Release 已存在

**解决方案**：
```bash
# 如果 v1.0.0 已存在，需要删除后重新创建
# 方案 1：在 GitHub 网页上删除 Release 和 Tag
# 方案 2：使用新版本号
git tag v1.0.1
git push origin v1.0.1
```

---

## 💡 实用技巧

### 技巧 1：查看构建进度

```bash
# 推送 tag 后立即查看
git push origin v1.0.0 && open https://github.com/你的用户名/WhisperApp.Avalona/actions
```

### 技巧 2：批量构建多个版本

```bash
# 为历史版本补充构建
git tag v0.9.0 HEAD~10
git tag v0.9.5 HEAD~5
git push origin --tags
```

### 技巧 3：删除错误的 tag

```bash
# 删除本地 tag
git tag -d v1.0.0

# 删除远程 tag
git push origin :refs/tags/v1.0.0
```

### 技巧 4：预发布版本

```bash
# 创建 beta 版本
git tag v1.0.0-beta
git push origin v1.0.0-beta

# 创建 rc 版本
git tag v1.0.0-rc1
git push origin v1.0.0-rc1
```

---

## 📊 构建时间参考

| 平台 | 大约耗时 |
|------|----------|
| Windows x64 | 3-5 分钟 |
| macOS Apple Silicon | 6-10 分钟 |
| macOS Intel | 6-10 分钟 |
| Linux x64 | 3-5 分钟 |
| **总计** | **15-25 分钟** |

> 注意：首次构建可能需要更长时间，因为需要下载依赖。

---

## 🎓 下一步学习

- 📚 查看 [完整文档](.github/workflows/README.md)
- 🔧 了解如何自定义工作流
- 🔐 学习代码签名
- 📝 设置自动版本号

---

## ✅ 检查清单

在推送 tag 发布前：

- [ ] 代码已提交并推送到 main 分支
- [ ] 本地测试通过
- [ ] 版本号符合语义化版本规范（v1.0.0）
- [ ] 更新了 CHANGELOG.md（如果有）
- [ ] 准备好 Release 说明

---

**🎉 现在你可以轻松使用 GitHub Actions 自动打包应用了！**

有问题？查看 [完整文档](.github/workflows/README.md) 或 [提交 Issue](https://github.com/你的用户名/WhisperApp.Avalona/issues)

