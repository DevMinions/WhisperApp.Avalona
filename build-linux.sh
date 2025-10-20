#!/bin/bash

# WhisperApp.Avalonia - Linux 打包脚本

echo "========================================"
echo "  WhisperApp - Linux 打包工具"
echo "========================================"
echo ""

# 发布应用
echo "正在发布应用..."
dotnet publish -c Release -r linux-x64 --self-contained

if [ $? -ne 0 ]; then
    echo "❌ 发布失败！"
    exit 1
fi

# 创建发布目录
PUBLISH_DIR="WhisperApp-Linux"
rm -rf "$PUBLISH_DIR"
mkdir -p "$PUBLISH_DIR"

# 复制文件
cp -r bin/Release/net9.0/linux-x64/publish/* "$PUBLISH_DIR/"

# 创建启动脚本
cat > "$PUBLISH_DIR/run.sh" << 'EOF'
#!/bin/bash
cd "$(dirname "$0")"
./WhisperApp.Avalonia
EOF

chmod +x "$PUBLISH_DIR/run.sh"
chmod +x "$PUBLISH_DIR/WhisperApp.Avalonia"

# 创建 .desktop 文件（用于桌面快捷方式）
cat > "$PUBLISH_DIR/whisperapp.desktop" << 'EOF'
[Desktop Entry]
Version=1.0
Type=Application
Name=WhisperApp
Comment=音频转录应用
Exec=./run.sh
Icon=audio-x-generic
Terminal=false
Categories=AudioVideo;Audio;
EOF

echo ""
echo "========================================"
echo "✅ 打包完成！"
echo ""
echo "生成的目录: $PUBLISH_DIR/"
echo ""
echo "运行应用:"
echo "  cd $PUBLISH_DIR"
echo "  ./run.sh"
echo ""
echo "或者:"
echo "  ./$PUBLISH_DIR/WhisperApp.Avalonia"
echo ""
echo "创建桌面快捷方式:"
echo "  cp $PUBLISH_DIR/whisperapp.desktop ~/.local/share/applications/"
echo "========================================"

