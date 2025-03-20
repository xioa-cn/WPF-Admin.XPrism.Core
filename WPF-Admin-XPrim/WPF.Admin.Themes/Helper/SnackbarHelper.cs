using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using WPF.Admin.Themes.Controls;

namespace WPF.Admin.Themes.Helper;

public class SnackbarHelper {
    private static Controls.Snackbar CreateSnackbar(string message, TimeSpan duration) {
        return new Controls.Snackbar {
            Message = message,
            Duration = duration,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Bottom,
            Margin = new Thickness(0, 0, 0, 20)
        };
    }

    private static void AddSnackbarToWindow(Window window, Controls.Snackbar snackbar) {
        // 创建或获取 AdornerLayer 作为顶层容器
        var adornerLayer = AdornerLayer.GetAdornerLayer(window);
        if (adornerLayer == null)
        {
            var container = window.Content as Panel;
            if (container == null)
            {
                var originalContent = window.Content;
                container = new Grid();
                window.Content = container;
                if (originalContent != null)
                {
                    (container as Grid).Children.Add(originalContent as UIElement);
                }
            }

            // 移除旧的Snackbar
            var existingSnackbars = new UIElement[container.Children.Count];
            container.Children.CopyTo(existingSnackbars, 0);
            foreach (var element in existingSnackbars)
            {
                if (element is Controls.Snackbar)
                {
                    container.Children.Remove(element);
                }
            }

            // 添加新的Snackbar
            Panel.SetZIndex(snackbar, 999999); // 设置最高层级
            container.Children.Add(snackbar);
        }
        else
        {
            // 使用 AdornerLayer 显示在最顶层
            var adorner = new SnackbarAdorner(window, snackbar);
            adornerLayer.Add(adorner);
        }
    }

    public static void Show(string message, long duration = 1000) {
        TimeSpan time = TimeSpan.FromMilliseconds(duration);
        var window = Application.Current.Windows.Cast<Window>()
            .FirstOrDefault(w => w.IsActive);
        if (window == null) return;

        if (!window.ShowInTaskbar) return;

        System.Diagnostics.Debug.WriteLine("找到了 当前显示的windows");
        window.Dispatcher.Invoke(() =>
        {
            try
            {
                var snackbar = CreateSnackbar(message, time);
                AddSnackbarToWindow(window, snackbar);
                snackbar.IsActive = true;
                System.Diagnostics.Debug.WriteLine("Snackbar已创建并激活");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"创建Snackbar时出错: {ex.Message}");
            }
        });
    }


    public static void Show(string message, string windowTitle, long duration = 1000) {
        var window = WindowHelper.FindWindowByTitle(windowTitle);
        if(window is null) return;
        TimeSpan time = TimeSpan.FromMilliseconds(duration);
        window.Dispatcher.Invoke(() =>
        {
            try
            {
                var snackbar = CreateSnackbar(message, time);
                AddSnackbarToWindow(window, snackbar);
                snackbar.IsActive = true;
                System.Diagnostics.Debug.WriteLine("Snackbar已创建并激活");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"创建Snackbar时出错: {ex.Message}");
            }
        });
    }
}