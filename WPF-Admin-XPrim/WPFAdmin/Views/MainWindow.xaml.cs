using System.ComponentModel;
using System.Windows;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using WPF.Admin.Models.Models;
using WPF.Admin.Themes.Converter;
using WPF.Admin.Themes.Helper;
using WPF.Admin.Themes.Themes;
using WPFAdmin.LoginModules;
using WPFAdmin.ViewModels;
using XPrism.Core.DataContextWindow;
using XPrism.Core.DI;
using XPrism.Core.Navigations;

namespace WPFAdmin.Views;

[XPrismViewModel(nameof(MainWindow))]
public partial class MainWindow {
    public MainWindow(INavigationService navigationService) {
        Dialog.Register(HcDialogMessageToken.DialogMainToken, this);
        InitializeComponent();
        navigationService.NavigateAsync("MainRegion/Main");


        // 窗口加载完成后注册热键
        Loaded += MainWindow_Loaded;
        // 窗口关闭前注销热键
        Closing += MainWindow_Closing;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e) {
        // 初始化热键管理器
        _hotKeyManager = new GlobalHotKey(this);

        try
        {
            // 注册 Ctrl+Alt+S 热键
            int id1 = _hotKeyManager.RegisterHotKey(
                GlobalHotKey.ModControl | GlobalHotKey.ModAlt,
                (uint)'S', () =>
                {
                    //App.MainShow();
                    SnackbarHelper.Show($"打开", "Admin.XPrism.Core", 5000);
                });
        }
        catch (Exception ex)
        {
            SnackbarHelper.Show($"热键注册失败: {ex.Message}");
        }
    }

    private GlobalHotKey _hotKeyManager;

    private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
        // 注销所有热键
        _hotKeyManager?.UnregisterAllHotKeys();
    }

    protected override void OnClosing(CancelEventArgs e) {
        base.OnClosing(e);
        this.CloseApplication();
    }

    private void CloseApplication() {
        App.DisposeAppResources();
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
                App.DisposeAppResources();
                this.CloseWindowWithFade();
                Environment.Exit(0);
                break;
            case CloseEnum.Notify:
                await Task.Delay(100);
                this.Visibility = Visibility.Hidden;
                break;
            case CloseEnum.None:
                break;
            case CloseEnum.Logout:
                GoBackLoginView();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void GoBackLoginView() {
        App.DisposeNotifyIconResources(); // 清理图标防止从图标打开主界面
        LoginAuthHelper.LoginUser = null; // 注销权限

        var login = XPrismIoc.Fetch<LoginWindow>();
        if (login is not null)
        {
            this.Visibility = Visibility.Hidden;
            login.Show();
        }
        else
        {
            Growl.ErrorGlobal("Not Found LoginWindow");
        }
    }
}