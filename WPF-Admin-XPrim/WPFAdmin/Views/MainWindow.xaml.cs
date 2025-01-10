using System.ComponentModel;
using XPrism.Core.DataContextWindow;
using XPrism.Core.Navigations;

namespace WPFAdmin.Views;

[XPrismViewModel(nameof(MainWindow))]
public partial class MainWindow {
    public MainWindow(INavigationService navigationService) {
        InitializeComponent();
        navigationService.NavigateAsync("MainRegion/Main");
    }

    protected override void OnClosing(CancelEventArgs e) {
        base.OnClosing(e);
        this.CloseApplication();
    }

    private void CloseApplication() {
        App.DisposeNotifyIcon();
        XPrism.Core.Co.CloseApplication.ShutdownApplication();
    }
}