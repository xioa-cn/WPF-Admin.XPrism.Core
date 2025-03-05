using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace PersonalityComponentModules.Views;

public partial class ReadTextView : Page {
    private SpeechSynthesizer _synthesizer;
    private bool _isPaused = false;
    private string _currentText = string.Empty;
    private int _currentPosition = 0;
    
    public ReadTextView() {
        InitializeComponent();
        
        // 初始化语音合成器
        InitializeSpeechSynthesizer();
        
        // 页面卸载时释放资源
        this.Unloaded += ReadTextView_Unloaded;
    }
    
    private void InitializeSpeechSynthesizer() {
        try {
            _synthesizer = new SpeechSynthesizer();
            
            // 设置事件处理程序
            _synthesizer.SpeakStarted += Synthesizer_SpeakStarted;
            _synthesizer.SpeakProgress += Synthesizer_SpeakProgress;
            _synthesizer.SpeakCompleted += Synthesizer_SpeakCompleted;
            
            // 加载可用的语音
            LoadAvailableVoices();
            
            // 设置初始属性
            _synthesizer.Rate = (int)RateSlider.Value;
            _synthesizer.Volume = (int)VolumeSlider.Value;
            
            // 添加滑块值变化事件处理程序
            RateSlider.ValueChanged += (s, e) => {
                if (_synthesizer != null)
                    _synthesizer.Rate = (int)RateSlider.Value;
            };
            
            VolumeSlider.ValueChanged += (s, e) => {
                if (_synthesizer != null)
                    _synthesizer.Volume = (int)VolumeSlider.Value;
            };
            
            // 添加语音选择事件处理程序
            VoiceComboBox.SelectionChanged += (s, e) => {
                if (_synthesizer != null && VoiceComboBox.SelectedItem != null)
                {
                    string selectedVoice = VoiceComboBox.SelectedItem.ToString();
                    _synthesizer.SelectVoice(selectedVoice);
                }
            };
        }
        catch (Exception ex) {
            MessageBox.Show($"初始化语音合成器失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    
    private void LoadAvailableVoices() {
        try {
            // 获取所有可用的语音
            var voices = _synthesizer.GetInstalledVoices()
                                    .Where(v => v.Enabled)
                                    .Select(v => v.VoiceInfo.Name)
                                    .ToList();
            
            // 添加到下拉列表
            VoiceComboBox.ItemsSource = voices;
            
            // 如果有可用的语音，选择第一个
            if (voices.Count > 0) {
                VoiceComboBox.SelectedIndex = 0;
                
                // 尝试选择中文语音（如果有）
                var chineseVoice = voices.FirstOrDefault(v => v.Contains("Chinese") || v.Contains("中文"));
                if (!string.IsNullOrEmpty(chineseVoice)) {
                    VoiceComboBox.SelectedItem = chineseVoice;
                }
            }
        }
        catch (Exception ex) {
            MessageBox.Show($"加载语音失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    
    private void StartButton_Click(object sender, RoutedEventArgs e) {
        try {
            if (_isPaused) {
                // 如果是暂停状态，继续朗读
                _synthesizer.Resume();
                _isPaused = false;
                StartButton.Content = "开始朗读";
            }
            else {
                // 获取要朗读的文本
                _currentText = InputTextBox.Text;
                
                if (string.IsNullOrEmpty(_currentText)) {
                    MessageBox.Show("请输入要朗读的文本", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                
                // 开始朗读
                _synthesizer.SpeakAsync(_currentText);
            }
            
            // 更新按钮状态
            StartButton.IsEnabled = false;
            PauseButton.IsEnabled = true;
            StopButton.IsEnabled = true;
        }
        catch (Exception ex) {
            MessageBox.Show($"朗读失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    
    private void PauseButton_Click(object sender, RoutedEventArgs e) {
        try {
            if (_isPaused) {
                // 如果已经暂停，继续朗读
                _synthesizer.Resume();
                PauseButton.Content = "暂停";
            }
            else {
                // 暂停朗读
                _synthesizer.Pause();
                PauseButton.Content = "继续";
            }
            
            _isPaused = !_isPaused;
            StartButton.IsEnabled = false;
        }
        catch (Exception ex) {
            MessageBox.Show($"暂停/继续失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    
    private void StopButton_Click(object sender, RoutedEventArgs e) {
        try {
            // 停止朗读
            _synthesizer.SpeakAsyncCancelAll();
            
            // 重置状态
            ResetUIState();
        }
        catch (Exception ex) {
            MessageBox.Show($"停止朗读失败: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    
    private void Synthesizer_SpeakStarted(object sender, SpeakStartedEventArgs e) {
        // 在UI线程上更新界面
        Dispatcher.Invoke(() => {
            StatusIndicator.Visibility = Visibility.Visible;
            StatusText.Text = "正在朗读...";
        });
    }
    
    private void Synthesizer_SpeakProgress(object sender, SpeakProgressEventArgs e) {
        // 在UI线程上更新界面，显示当前朗读位置
        Dispatcher.Invoke(() => {
            _currentPosition = e.CharacterPosition;
            StatusText.Text = $"正在朗读... ({e.CharacterPosition}/{_currentText.Length})";
            
            // 可以选择在文本框中高亮当前朗读的文本
            // 这需要更复杂的实现，此处省略
        });
    }
    
    private void Synthesizer_SpeakCompleted(object sender, SpeakCompletedEventArgs e) {
        // 在UI线程上更新界面
        Dispatcher.Invoke(() => {
            ResetUIState();
            
            if (e.Cancelled) {
                StatusText.Text = "朗读已取消";
            }
            else if (e.Error != null) {
                StatusText.Text = $"朗读出错: {e.Error.Message}";
            }
            else {
                StatusText.Text = "朗读完成";
                
                // 短暂显示完成状态，然后隐藏
                DispatcherTimer timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(2);
                timer.Tick += (s, args) => {
                    StatusIndicator.Visibility = Visibility.Collapsed;
                    timer.Stop();
                };
                timer.Start();
            }
        });
    }
    
    private void ResetUIState() {
        // 重置UI状态
        StartButton.Content = "开始朗读";
        StartButton.IsEnabled = true;
        PauseButton.Content = "暂停";
        PauseButton.IsEnabled = false;
        StopButton.IsEnabled = false;
        _isPaused = false;
    }
    
    private void ReadTextView_Unloaded(object sender, RoutedEventArgs e) {
        // 释放资源
        if (_synthesizer != null) {
            _synthesizer.SpeakAsyncCancelAll();
            _synthesizer.Dispose();
            _synthesizer = null;
        }
    }
}