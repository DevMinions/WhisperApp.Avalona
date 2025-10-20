@echo off
chcp 65001 >nul
echo ========================================
echo   WhisperApp - Windows 打包工具
echo ========================================
echo.

echo 正在发布应用...
dotnet publish -c Release -r win-x64 --self-contained

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo ❌ 发布失败！
    pause
    exit /b 1
)

echo.
echo 正在创建发布目录...

set PUBLISH_DIR=WhisperApp-Windows
if exist "%PUBLISH_DIR%" rmdir /s /q "%PUBLISH_DIR%"
mkdir "%PUBLISH_DIR%"

xcopy /E /I /Y "bin\Release\net9.0\win-x64\publish\*" "%PUBLISH_DIR%\"

echo.
echo 创建启动脚本...

echo @echo off > "%PUBLISH_DIR%\运行应用.bat"
echo chcp 65001 ^>nul >> "%PUBLISH_DIR%\运行应用.bat"
echo echo 正在启动 WhisperApp... >> "%PUBLISH_DIR%\运行应用.bat"
echo start WhisperApp.Avalonia.exe >> "%PUBLISH_DIR%\运行应用.bat"

echo.
echo ========================================
echo ✅ 打包完成！
echo.
echo 生成的目录: %PUBLISH_DIR%\
echo.
echo 运行应用:
echo   1. 进入 %PUBLISH_DIR% 目录
echo   2. 双击 WhisperApp.Avalonia.exe
echo      或双击 运行应用.bat
echo ========================================
echo.
pause

