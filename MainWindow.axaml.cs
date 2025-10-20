using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Threading;

namespace WhisperApp.Avalonia;

public partial class MainWindow : Window
{
    private readonly HttpClient _httpClient;
    private CancellationTokenSource? _cancellationTokenSource;
    private string? _selectedFilePath;
    private string? _currentTranscriptionText;
    private const string BaseUrl = "https://api.assemblyai.com";

    public MainWindow()
    {
        InitializeComponent();
        _httpClient = new HttpClient();
        
        // Enable transcribe button when both API key and file are provided
        ApiKeyTextBox.PropertyChanged += (s, e) =>
        {
            if (e.Property.Name == nameof(TextBox.Text))
            {
                UpdateTranscribeButtonState();
            }
        };
    }

    private void UpdateTranscribeButtonState()
    {
        TranscribeButton.IsEnabled = !string.IsNullOrWhiteSpace(ApiKeyTextBox.Text) 
                                     && !string.IsNullOrWhiteSpace(_selectedFilePath);
    }

    private async void BrowseButton_Click(object? sender, RoutedEventArgs e)
    {
        var storage = StorageProvider;
        
        var files = await storage.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "选择音频文件",
            AllowMultiple = false,
            FileTypeFilter = new[]
            {
                new FilePickerFileType("音频文件")
                {
                    Patterns = new[] { "*.mp3", "*.wav", "*.m4a", "*.flac", "*.ogg", "*.opus", "*.webm" }
                },
                new FilePickerFileType("视频文件")
                {
                    Patterns = new[] { "*.mp4", "*.avi", "*.mov", "*.mkv" }
                },
                new FilePickerFileType("所有文件")
                {
                    Patterns = new[] { "*.*" }
                }
            }
        });

        if (files.Count > 0)
        {
            _selectedFilePath = files[0].Path.LocalPath;
            FilePathTextBox.Text = _selectedFilePath;
            UpdateStatus($"已选择文件: {Path.GetFileName(_selectedFilePath)}");
            UpdateTranscribeButtonState();
        }
    }

    private async void TranscribeButton_Click(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(ApiKeyTextBox.Text))
        {
            await ShowMessageBox("请输入您的 AssemblyAI API 密钥。", "需要 API 密钥");
            return;
        }

        if (string.IsNullOrWhiteSpace(_selectedFilePath))
        {
            await ShowMessageBox("请选择一个音频文件。", "需要文件");
            return;
        }

        try
        {
            _cancellationTokenSource = new CancellationTokenSource();
            SetUIState(isTranscribing: true);
            ResultTextBox.Text = "正在开始转录...";
            
            await TranscribeAudioAsync(_selectedFilePath, ApiKeyTextBox.Text, _cancellationTokenSource.Token);
        }
        catch (OperationCanceledException)
        {
            UpdateStatus("转录已取消。");
            ResultTextBox.Text = "转录已被取消。";
        }
        catch (Exception ex)
        {
            UpdateStatus($"错误: {ex.Message}");
            await ShowMessageBox($"发生错误: {ex.Message}", "错误");
            ResultTextBox.Text = $"错误: {ex.Message}";
        }
        finally
        {
            SetUIState(isTranscribing: false);
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }
    }

    private async Task TranscribeAudioAsync(string filePath, string apiKey, CancellationToken cancellationToken)
    {
        // Set authorization header
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("authorization", apiKey);

        // Step 1: Upload the audio file
        UpdateStatus("正在上传音频文件...");
        string uploadUrl = await UploadFileAsync(filePath, cancellationToken);

        // Step 2: Submit for transcription
        UpdateStatus("正在提交转录请求...");
        string transcriptId = await SubmitTranscriptionAsync(uploadUrl, cancellationToken);

        // Step 3: Poll for completion
        UpdateStatus("正在处理转录...");
        string transcriptionText = await PollTranscriptionAsync(transcriptId, cancellationToken);

        // Step 4: Display result
        _currentTranscriptionText = transcriptionText;
        ResultTextBox.Text = transcriptionText;
        ExportButton.IsEnabled = true;
        UpdateStatus("转录成功完成！");
        
        await ShowMessageBox("转录成功完成！", "成功");
    }

    private async Task<string> UploadFileAsync(string filePath, CancellationToken cancellationToken)
    {
        using var fileStream = File.OpenRead(filePath);
        using var content = new StreamContent(fileStream);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

        var response = await _httpClient.PostAsync($"{BaseUrl}/v2/upload", content, cancellationToken);
        
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new Exception($"上传失败: {response.StatusCode} - {errorContent}");
        }

        var responseData = await response.Content.ReadFromJsonAsync<UploadResponse>(cancellationToken);
        
        if (responseData?.UploadUrl == null)
        {
            throw new Exception("上传响应中未包含上传 URL。");
        }

        return responseData.UploadUrl;
    }

    private async Task<string> SubmitTranscriptionAsync(string audioUrl, CancellationToken cancellationToken)
    {
        var requestData = new { audio_url = audioUrl };
        var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/v2/transcript", requestData, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new Exception($"转录提交失败: {response.StatusCode} - {errorContent}");
        }

        var responseData = await response.Content.ReadFromJsonAsync<TranscriptResponse>(cancellationToken);
        
        if (responseData?.Id == null)
        {
            throw new Exception("转录响应中未包含 ID。");
        }

        return responseData.Id;
    }

    private async Task<string> PollTranscriptionAsync(string transcriptId, CancellationToken cancellationToken)
    {
        var pollingEndpoint = $"{BaseUrl}/v2/transcript/{transcriptId}";
        
        while (!cancellationToken.IsCancellationRequested)
        {
            var response = await _httpClient.GetAsync(pollingEndpoint, cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new Exception($"轮询失败: {response.StatusCode} - {errorContent}");
            }

            var transcript = await response.Content.ReadFromJsonAsync<TranscriptStatusResponse>(cancellationToken);

            if (transcript == null)
            {
                throw new Exception("无法解析转录状态。");
            }

            if (transcript.Status == "completed")
            {
                if (string.IsNullOrEmpty(transcript.Text))
                {
                    throw new Exception("转录已完成但未返回文本。");
                }
                return transcript.Text;
            }
            else if (transcript.Status == "error")
            {
                throw new Exception($"转录失败: {transcript.Error ?? "未知错误"}");
            }

            // Wait before polling again
            await Task.Delay(3000, cancellationToken);
        }

        throw new OperationCanceledException();
    }

    private void CancelButton_Click(object? sender, RoutedEventArgs e)
    {
        _cancellationTokenSource?.Cancel();
        UpdateStatus("正在取消转录...");
    }

    private async void ExportButton_Click(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_currentTranscriptionText))
        {
            await ShowMessageBox("没有可导出的转录内容。", "导出错误");
            return;
        }

        var storage = StorageProvider;
        
        var file = await storage.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "导出转录",
            SuggestedFileName = $"转录_{DateTime.Now:yyyyMMdd_HHmmss}.txt",
            FileTypeChoices = new[]
            {
                new FilePickerFileType("文本文件")
                {
                    Patterns = new[] { "*.txt" }
                },
                new FilePickerFileType("所有文件")
                {
                    Patterns = new[] { "*.*" }
                }
            }
        });

        if (file != null)
        {
            try
            {
                await File.WriteAllTextAsync(file.Path.LocalPath, _currentTranscriptionText);
                UpdateStatus($"转录已导出到: {file.Path.LocalPath}");
                await ShowMessageBox("转录导出成功！", "导出成功");
            }
            catch (Exception ex)
            {
                await ShowMessageBox($"导出转录失败: {ex.Message}", "导出错误");
            }
        }
    }

    private void SetUIState(bool isTranscribing)
    {
        Dispatcher.UIThread.Post(() =>
        {
            ApiKeyTextBox.IsEnabled = !isTranscribing;
            BrowseButton.IsEnabled = !isTranscribing;
            TranscribeButton.IsEnabled = !isTranscribing;
            CancelButton.IsEnabled = isTranscribing;
            ProgressBar.IsVisible = isTranscribing;
            ProgressBar.IsIndeterminate = isTranscribing;
        });
    }

    private void UpdateStatus(string message)
    {
        Dispatcher.UIThread.Post(() =>
        {
            StatusTextBlock.Text = message;
        });
    }

    private async Task ShowMessageBox(string message, string title)
    {
        var messageBox = new Window
        {
            Title = title,
            Width = 400,
            Height = 150,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false
        };

        var panel = new StackPanel
        {
            Margin = new global::Avalonia.Thickness(20)
        };

        panel.Children.Add(new TextBlock
        {
            Text = message,
            TextWrapping = global::Avalonia.Media.TextWrapping.Wrap,
            Margin = new global::Avalonia.Thickness(0, 0, 0, 20)
        });

        var button = new Button
        {
            Content = "确定",
            HorizontalAlignment = global::Avalonia.Layout.HorizontalAlignment.Center,
            Padding = new global::Avalonia.Thickness(30, 8),
            Classes = { "ModernButton" }
        };

        button.Click += (s, e) => messageBox.Close();

        panel.Children.Add(button);
        messageBox.Content = panel;

        await messageBox.ShowDialog(this);
    }

    // Data models for JSON serialization
    private class UploadResponse
    {
        [JsonPropertyName("upload_url")]
        public string? UploadUrl { get; set; }
    }

    private class TranscriptResponse
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }
    }

    private class TranscriptStatusResponse
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }
        
        [JsonPropertyName("status")]
        public string? Status { get; set; }
        
        [JsonPropertyName("text")]
        public string? Text { get; set; }
        
        [JsonPropertyName("error")]
        public string? Error { get; set; }
    }
}
