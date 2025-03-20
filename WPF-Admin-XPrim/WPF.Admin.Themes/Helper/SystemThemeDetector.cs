using Microsoft.Win32;

namespace WPF.Admin.Themes.Helper;

/// <summary>
/// 获取windows主题
/// </summary>
public class SystemThemeDetector {
            private bool isDarkMode;
        public event EventHandler<bool> ThemeChanged;

        public SystemThemeDetector()
        {
            isDarkMode = IsSystemInDarkMode();
            SystemEvents.UserPreferenceChanged += SystemEvents_UserPreferenceChanged;
        }

        public bool IsDarkMode => isDarkMode;
        
        private void SystemEvents_UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            if (e.Category == UserPreferenceCategory.General || e.Category == UserPreferenceCategory.VisualStyle)
            {
                var newDarkMode = IsSystemInDarkMode();
                if (newDarkMode != isDarkMode)
                {
                    isDarkMode = newDarkMode;
                    ThemeChanged?.Invoke(this, isDarkMode);
                }
            }
        }

        private bool IsSystemInDarkMode()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize"))
                {
                    if (key != null)
                    {
                        // 检查应用主题设置
                        object appsTheme = key.GetValue("AppsUseLightTheme");
                        if (appsTheme != null)
                        {
                            return (int)appsTheme == 0;
                        }

                        // 检查系统主题设置
                        object systemTheme = key.GetValue("SystemUsesLightTheme");
                        if (systemTheme != null)
                        {
                            return (int)systemTheme == 0;
                        }
                    }
                }

                // 尝试检查高对比度主题
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Accessibility\HighContrast"))
                {
                    if (key != null)
                    {
                        string highContrast = key.GetValue("High Contrast Scheme") as string;
                        if (!string.IsNullOrEmpty(highContrast) && !highContrast.Equals("None"))
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // 记录异常信息，但继续使用默认主题
                System.Diagnostics.Debug.WriteLine($"Theme detection error: {ex.Message}");
            }
            return false;
        }

        public void Dispose()
        {
            SystemEvents.UserPreferenceChanged -= SystemEvents_UserPreferenceChanged;
        }
}