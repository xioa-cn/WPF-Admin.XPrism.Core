using System.ComponentModel;
using System.Windows;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using WPF.Admin.Models.Models;
using WPF.Admin.Themes.Themes;
using WPFAdmin.ViewModels;
using XPrism.Core.DataContextWindow;
using XPrism.Core.Navigations;

namespace WPFAdmin.Views;

[XPrismViewModel(nameof(MainWindow))]
public partial class MainWindow {
    public MainWindow(INavigationService navigationService) {
        Dialog.Register(HcDialogMessageToken.DialogMainToken, this);
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

    private void MiniSize_Click(object sender, RoutedEventArgs e) {
        this.WindowState = WindowState.Minimized;
    }

    private void MaxSize_Click(object sender, RoutedEventArgs e) {
        if (this.WindowState == WindowState.Normal)
        {
            this.WindowState = WindowState.Maximized;
        }

        else if (this.WindowState == WindowState.Maximized)
        {
            this.WindowState = WindowState.Normal;
        }
    }
    private NotifyIconView? _notifyIconView;
    private async void Close_Click(object sender, RoutedEventArgs e) {
        _notifyIconView ??= new NotifyIconView();

        var closeEnum = CloseEnum.None;
        var dialog = Dialog.Show(_notifyIconView, HcDialogMessageToken.DialogMainToken);
        await dialog.Initialize<NotifyIconViewModel>(
            vm => { }).GetResultAsync<CloseEnum>().ContinueWith(re => { closeEnum = re.Result; });

        switch (closeEnum)
        {
            case CloseEnum.Close:
                App.DisposeNotifyIcon();
                this.CloseWindowWithFade();
                Environment.Exit(0);
                break;
            case CloseEnum.Notify:
                await Task.Delay(100);
                this.Visibility = Visibility.Hidden;
                break;
            case CloseEnum.None:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}