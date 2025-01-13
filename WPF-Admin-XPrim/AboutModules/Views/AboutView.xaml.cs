using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace AboutModules.Views;

public partial class AboutView : UserControl {
    public AboutView() {
        InitializeComponent();
    }

    private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e) {
        // 使用默认浏览器打开链接
        Process.Start(new ProcessStartInfo
        {
            FileName = e.Uri.AbsoluteUri,
            UseShellExecute = true
        });

        // 标记事件已处理
        e.Handled = true;
    }
}