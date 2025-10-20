#!/bin/bash

# WhisperApp.Avalonia - macOS 打包脚本

echo "========================================"
echo "  WhisperApp - macOS 打包工具"
echo "========================================"
echo ""

# 选择架构
echo "请选择目标架构:"
echo "1) Apple Silicon (M1/M2/M3) - arm64"
echo "2) Intel - x64"
echo "3) 通用包 (两者都打包)"
echo ""
read -p "请输入选择 (1-3): " choice

case $choice in
    1)
        RID="osx-arm64"
        ARCH_NAME="AppleSilicon"
        ;;
    2)
        RID="osx-x64"
        ARCH_NAME="Intel"
        ;;
    3)
        RID="both"
        ;;
    *)
        echo "无效选择，退出。"
        exit 1
        ;;
esac

# 发布应用
echo ""
echo "正在发布应用..."
echo ""

if [ "$RID" = "both" ]; then
    echo "正在构建 Apple Silicon 版本..."
    dotnet publish -c Release -r osx-arm64 --self-contained -p:PublishSingleFile=false
    
    echo ""
    echo "正在构建 Intel 版本..."
    dotnet publish -c Release -r osx-x64 --self-contained -p:PublishSingleFile=false
    
    # 创建两个 .app 包
    echo ""
    echo "正在创建 Apple Silicon .app 包..."
    create_app_bundle "osx-arm64" "AppleSilicon"
    
    echo ""
    echo "正在创建 Intel .app 包..."
    create_app_bundle "osx-x64" "Intel"
    
    echo ""
    echo "========================================"
    echo "✅ 打包完成！"
    echo ""
    echo "生成的应用:"
    echo "  - WhisperApp-AppleSilicon.app (Apple Silicon M1/M2/M3)"
    echo "  - WhisperApp-Intel.app (Intel 处理器)"
    echo "========================================"
else
    dotnet publish -c Release -r $RID --self-contained -p:PublishSingleFile=false
    
    if [ $? -ne 0 ]; then
        echo "❌ 发布失败！"
        exit 1
    fi
    
    # 创建 .app 包
    echo ""
    echo "正在创建 .app 包..."
    create_app_bundle "$RID" "$ARCH_NAME"
    
    echo ""
    echo "========================================"
    echo "✅ 打包完成！"
    echo ""
    echo "生成的应用: WhisperApp-${ARCH_NAME}.app"
    echo ""
    echo "运行应用:"
    echo "  open WhisperApp-${ARCH_NAME}.app"
    echo ""
    echo "或者双击 WhisperApp-${ARCH_NAME}.app 运行"
    echo "========================================"
fi

# 函数：创建 .app 包
create_app_bundle() {
    local rid=$1
    local arch_name=$2
    local app_name="WhisperApp-${arch_name}.app"
    
    # 清理旧的 .app
    rm -rf "$app_name"
    
    # 创建目录结构
    mkdir -p "$app_name/Contents/MacOS"
    mkdir -p "$app_name/Contents/Resources"
    
    # 复制文件
    cp -r "bin/Release/net9.0/${rid}/publish/"* "$app_name/Contents/MacOS/"
    
    # 创建 Info.plist
    cat > "$app_name/Contents/Info.plist" << 'EOF'
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
</dict>
</plist>
EOF
    
    # 设置可执行权限
    chmod +x "$app_name/Contents/MacOS/WhisperApp.Avalonia"
    
    # 移除扩展属性（允许运行未签名应用）
    xattr -cr "$app_name"
    
    echo "✅ 创建 $app_name 成功！"
}

