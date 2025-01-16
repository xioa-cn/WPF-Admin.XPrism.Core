using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using WPF.Admin.Models;

namespace WPF.Admin.Themes;

public partial class ThemeManager : BindableBase
{
    private static ThemeManager? _instance;
    public static ThemeManager Instance => _instance ??= new ThemeManager();

    [ObservableProperty] private bool _isDarkTheme;


    public ThemeManager()
    {
        // 默认使用深色主题
        IsDarkTheme = false;
        ApplyTheme();
    }

    partial void OnIsDarkThemeChanged(bool value)
    {
        ApplyTheme();
    }

    /***
     * 新建页面时 配合主题使用
     *
     * 在每个页面的根元素（Page、UserControl等）添加：Background="{DynamicResource Background.Brush}"
     *
     * 对于所有文本元素（TextBlock、TextBox等）使用
     * Foreground="{DynamicResource Text.Primary.Brush}"  <!-- 主要文本 -->
     * Foreground="{DynamicResource Text.Secondary.Brush}"  <!-- 次要文本 -->
     *
     * 对于所有边框和分隔线使用：
     * BorderBrush="{DynamicResource Border.Brush}"
     *
     * 对于面板和容器使用：
     * Background="{DynamicResource Background.Brush}"  <!-- 主背景 -->
     * Background="{DynamicResource Surface.Brush}"  <!-- 次要背景 -->
     *
     * 对于按钮和交互元素使用：
     * Background="{DynamicResource Primary.Brush}"  <!-- 主要按钮 -->
     * Background="{DynamicResource Secondary.Brush}"  <!-- 次要按钮 -->
     *
     * 对于输入控件（TextBox、ComboBox等）使用：
     * Background="{DynamicResource Background.Brush}"
     * Foreground="{DynamicResource Text.Primary.Brush}"
     * BorderBrush="{DynamicResource Border.Brush}"
     *
     */


    // HandyControl主题资源
    private static readonly ResourceDictionary HcLightTheme = new()
    { 
        Source = new Uri("pack://application:,,,/HandyControl;component/Themes/SkinDefault.xaml") 
    };
    private static readonly ResourceDictionary HcDarkTheme = new()
    { 
        Source = new Uri("pack://application:,,,/HandyControl;component/Themes/SkinDark.xaml") 
    };

    // 自定义主题资源
    private static readonly ResourceDictionary CustomLightTheme = new()
    { 
        Source = new Uri("pack://application:,,,/WPF.Admin.Themes;component/Themes/CyanTheme.xaml") 
    };
    private static readonly ResourceDictionary CustomDarkTheme = new()
    { 
        Source = new Uri("pack://application:,,,/WPF.Admin.Themes;component/Themes/DarkCyanTheme.xaml") 
    };
    private void UpdateThemeResource(Collection<ResourceDictionary> resources, 
        ResourceDictionary lightTheme, ResourceDictionary darkTheme,bool hc=false)
    {
        if (hc)
        {
            // 1. 先移除所有HandyControl相关资源
            var hcResources = resources.Where(x => 
                x.Source?.ToString().Contains("/HandyControl;component/") ?? false).ToList();
            foreach (var resource in hcResources)
            {
                resources.Remove(resource);
            }
            // 2. 重新添加HandyControl基础资源
            resources.Add(new ResourceDictionary 
            { 
                Source = new Uri("pack://application:,,,/HandyControl;component/Themes/Theme.xaml") 
            });
        }
       

        // 添加新主题
        resources.Add(IsDarkTheme ? darkTheme : lightTheme);
    }
    private void ApplyTheme()
    {
        var app = Application.Current;
        if (app == null) return;

        Application.Current.Dispatcher.Invoke(() =>
        {
            var resources = app.Resources.MergedDictionaries;

            // 更新HandyControl主题
            UpdateThemeResource(resources, HcLightTheme, HcDarkTheme,true);

            // 更新自定义主题
            UpdateThemeResource(resources, CustomLightTheme, CustomDarkTheme);

            // 更新窗口背景
            UpdateWindowBackground();
        });
    }

    private void UpdateWindowBackground() {
        if (Application.Current.MainWindow?.Template.FindName("MainBorder", 
                Application.Current.MainWindow) is Border border)
        {
            border.Background = Application.Current.Resources["Background.Brush"] as SolidColorBrush;
        }
    }

    private static ResourceDictionary? _currentTheme;

    public static void UseTheme(string content)
    {
        var app = Application.Current;
        if (app == null) return;

        // 移除当前主题（如果存在）
        if (_currentTheme != null)
        {
            app.Resources.MergedDictionaries.Remove(_currentTheme);
        }

        var themePath =
            $"pack://application:,,,/WPF.Admin.Themes;component/Themes/{content}Theme.xaml";
        _currentTheme = new ResourceDictionary { Source = new Uri(themePath, UriKind.Absolute) };
        app.Resources.MergedDictionaries.Add(_currentTheme);
        // 更新窗口背景色
        if (Application.Current.MainWindow != null)
        {
            var border =
                Application.Current.MainWindow.Template.FindName("MainBorder",
                    Application.Current.MainWindow) as Border;
            if (border != null)
            {
                border.Background = (SolidColorBrush)Application.Current.Resources["Background.Brush"];
            }
        }
    }
}