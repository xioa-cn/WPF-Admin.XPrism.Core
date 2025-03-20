using WPF.Admin.Service.Services;
using WPF.Admin.Themes;
using WPF.Admin.Themes.Helper;
using WPFAdmin.Config;

namespace WPFAdmin;

public partial class App {
    /// <summary>
    /// 应用跟随系统颜色
    /// </summary>
    public void SystemTheme() {
        if (!Configs.Default.UseSystemTheme)
        {
            // 如果没有打开跟随系统 则默认使用浅色
            ThemeManager.Instance.IsDarkTheme = false;
            return;
        }
        SystemThemeDetector systemThemeDetector = new SystemThemeDetector();
        ThemeManager.Instance.IsDarkTheme = systemThemeDetector.IsDarkMode;
        systemThemeDetector.ThemeChanged += (sender, b) =>
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                ThemeManager.Instance.IsDarkTheme = b;
            });
        };
    }
}