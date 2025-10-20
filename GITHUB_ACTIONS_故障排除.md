# GitHub Actions 常见错误及解决方案

## 🔥 403 错误 - Release 创建失败

### ❌ 错误信息

```
⚠️ GitHub release failed with status: 403
undefined
Error: Too many retries.
```

### 🎯 原因

GitHub Actions 的 `GITHUB_TOKEN` 缺少创建 Release 的权限。

### ✅ 解决方案（已修复）

我已经在 `.github/workflows/build-release.yml` 中添加了必要的权限配置：

```yaml
permissions:
  contents: write
  packages: write
```

### 📋 还需要检查的地方

#### 1. 检查仓库设置（重要！）

进入你的 GitHub 仓库，检查 Actions 权限：

1. 打开仓库 → **Settings** (设置)
2. 左侧菜单找到 **Actions** → **General**
3. 向下滚动到 **"Workflow permissions"** 部分
4. 确保选择了：
   - ✅ **"Read and write permissions"** (读写权限)
   
   或者
   
   - ⚠️ 如果选择了 "Read repository contents and packages permissions"，需要改为上面的选项

5. 点击 **Save** 保存

#### 图示说明：

```
Settings → Actions → General

Workflow permissions
┌─────────────────────────────────────────────┐
│ ⭕ Read repository contents and packages    │
│    permissions                              │
│                                             │
│ ⦿ Read and write permissions               │  ← 选择这个
│   Allow GitHub Actions to create and       │
│   approve pull requests                     │
│   ☐ Allow GitHub Actions to create and     │
│      approve pull requests                  │
└─────────────────────────────────────────────┘

[Save]
```

#### 2. 重新运行工作流

修复后有两种方式：

**方式 A：重新运行失败的工作流**

1. 进入 **Actions** 页面
2. 点击失败的工作流运行
3. 点击右上角的 **"Re-run all jobs"** (重新运行所有任务)

**方式 B：推送新的提交触发**

```bash
# 提交权限修复
git add .github/workflows/build-release.yml
git commit -m "修复：添加 Release 创建权限"
git push

# 删除旧的 tag（如果需要）
git tag -d v1.0.0
git push origin :refs/tags/v1.0.0

# 重新创建 tag
git tag v1.0.0
git push origin v1.0.0
```

---

## 🔍 其他常见错误

### 错误 2：404 Not Found

**错误信息**：
```
Error: Resource not accessible by integration
```

**原因**：仓库不存在或无访问权限

**解决方案**：
1. 确认仓库 URL 正确
2. 检查是否为私有仓库
3. 确认 Actions 已启用

### 错误 3：编译失败

**错误信息**：
```
error CS0246: The type or namespace name 'xxx' could not be found
```

**原因**：代码编译错误或缺少依赖

**解决方案**：
```bash
# 本地测试编译
dotnet restore
dotnet build

# 如果本地成功，检查 .NET 版本是否匹配
# 查看工作流中的版本：
# DOTNET_VERSION: '9.0.x'
```

### 错误 4：Tag 格式错误

**错误信息**：
工作流没有触发

**原因**：Tag 格式不匹配

**解决方案**：
```bash
# ❌ 错误格式
git tag 1.0.0
git tag release-1.0.0

# ✅ 正确格式
git tag v1.0.0
git tag v1.0.0-beta
git tag v2.1.3-rc.1
```

### 错误 5：文件路径错误

**错误信息**：
```
Error: Unable to find file 'artifacts/windows-x64/WhisperApp-Windows-x64.zip'
```

**原因**：构建产物路径不正确

**解决方案**：
1. 检查每个构建 job 的输出
2. 确认 artifact 名称匹配
3. 查看构建日志中的文件列表

### 错误 6：超时错误

**错误信息**：
```
Error: The operation was canceled.
```

**原因**：构建时间超过 6 小时（GitHub 限制）

**解决方案**：
- 优化构建时间
- 使用缓存
- 减少并行任务

---

## 🛠️ 调试技巧

### 1. 查看详细日志

在工作流文件中添加调试输出：

```yaml
- name: 🐛 Debug - List files
  run: |
    echo "Current directory:"
    pwd
    echo "Files:"
    ls -la
    echo "Artifacts:"
    ls -la artifacts/ || true
```

### 2. 启用 Debug 日志

在仓库中设置 Secrets：

1. Settings → Secrets and variables → Actions
2. 添加 Secret：
   - Name: `ACTIONS_STEP_DEBUG`
   - Value: `true`

### 3. 使用 act 本地测试

安装 [act](https://github.com/nektos/act) 在本地运行 GitHub Actions：

```bash
# 安装 act
# Windows (使用 Chocolatey)
choco install act-cli

# macOS
brew install act

# 运行工作流
act -j build-windows
```

---

## 📊 权限说明

### GitHub Actions 支持的权限

```yaml
permissions:
  actions: write          # 管理 Actions
  checks: write           # 管理检查
  contents: write         # 读写仓库内容（创建 Release 需要）
  deployments: write      # 管理部署
  discussions: write      # 管理讨论
  issues: write           # 管理 Issues
  packages: write         # 发布包
  pages: write            # 管理 Pages
  pull-requests: write    # 管理 PR
  repository-projects: write  # 管理项目
  security-events: write  # 安全事件
  statuses: write         # 提交状态
```

### 创建 Release 需要的最小权限

```yaml
permissions:
  contents: write  # 必需：创建 Release 和上传文件
```

---

## ✅ 检查清单

在推送 tag 前检查：

- [ ] 工作流文件中已添加 `permissions: contents: write`
- [ ] 仓库设置中启用了 "Read and write permissions"
- [ ] 本地代码可以成功编译：`dotnet build`
- [ ] Tag 格式正确（v1.0.0）
- [ ] 所有文件已提交并推送
- [ ] Actions 已启用（Settings → Actions → General）

---

## 🔄 完整修复流程

```bash
# 1. 提交权限修复
git add .github/workflows/build-release.yml
git commit -m "修复：添加 GitHub Actions 权限"
git push

# 2. 去 GitHub 检查仓库设置
# Settings → Actions → General → Workflow permissions
# 选择 "Read and write permissions"

# 3. 删除旧的失败 tag（如果有）
git tag -d v1.0.0
git push origin :refs/tags/v1.0.0

# 4. 重新创建 tag
git tag v1.0.0
git push origin v1.0.0

# 5. 查看 Actions 页面
# https://github.com/你的用户名/WhisperApp.Avalona/actions
```

---

## 📞 仍然失败？

如果按照上述步骤仍然失败，请检查：

### 1. 仓库是否为 Fork？

如果这是一个 fork 的仓库，可能需要：
- 在原仓库的 Settings 中启用 Actions
- 或者使用个人 Access Token

### 2. 是否为组织仓库？

组织仓库可能有额外的权限限制：
- 联系组织管理员
- 检查组织级别的 Actions 设置

### 3. 使用个人 Access Token（高级）

如果默认 GITHUB_TOKEN 不够用：

```yaml
- name: 🎉 Create Release
  uses: softprops/action-gh-release@v1
  with:
    # ...
  env:
    GITHUB_TOKEN: ${{ secrets.PERSONAL_ACCESS_TOKEN }}
```

创建 Personal Access Token：
1. GitHub → Settings → Developer settings
2. Personal access tokens → Tokens (classic)
3. Generate new token
4. 勾选 `repo` 权限
5. 复制 token
6. 在仓库 Settings → Secrets 中添加

---

## 💡 预防措施

### 在新仓库中始终设置：

1. **启用 Actions**
   - Settings → Actions → General
   - Allow all actions and reusable workflows

2. **设置写权限**
   - Settings → Actions → General
   - Workflow permissions: "Read and write permissions"

3. **添加权限配置**
   ```yaml
   permissions:
     contents: write
   ```

---

## 🎉 成功标志

当一切正常时，你会看到：

```
Run softprops/action-gh-release@v1
👩‍🏭 Creating new GitHub release for tag v1.0.0...
⬆️ Uploading WhisperApp-Windows-x64.zip...
⬆️ Uploading WhisperApp-macOS-AppleSilicon.zip...
⬆️ Uploading WhisperApp-macOS-Intel.zip...
⬆️ Uploading WhisperApp-Linux-x64.tar.gz...
✅ Release created successfully!
```

然后在 Releases 页面就能看到你的发布了！

---

**🔧 问题解决了吗？如果还有问题，请查看 GitHub Actions 的详细日志！**

