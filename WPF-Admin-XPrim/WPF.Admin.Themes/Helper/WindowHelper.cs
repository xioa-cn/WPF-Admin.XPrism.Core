using System.Windows;
using System.Windows.Automation;
using System.Windows.Interop;

namespace WPF.Admin.Themes.Helper
{
    public static class WindowHelper
    {
        // 根据窗口标题查找窗口
        public static Window? FindWindowByTitle(string title)
        {
            // 获取桌面元素
            AutomationElement desktop = AutomationElement.RootElement;
            // 在桌面元素中查找名称为 title 的窗口
            AutomationElement window = desktop.FindFirst(
                TreeScope.Children,
                new PropertyCondition(AutomationElement.NameProperty, title));

            if (window != null)
            {
                // 获取窗口句柄
                IntPtr hwnd = new IntPtr(window.Current.NativeWindowHandle);
                // 可以通过句柄获取 Window 对象
                return HwndSource.FromHwnd(hwnd)?.RootVisual as Window;
            }

            return null;
        }
    }
}
