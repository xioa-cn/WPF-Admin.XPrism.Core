using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Hardcodet.Wpf.TaskbarNotification;
using WPF.Admin.Models.Models;
using WPFAdmin.ViewModels;
using WPFAdmin.Views;
using XPrism.Core.DI;

namespace WPFAdmin;

public partial class App
{
    private static TaskbarIcon? _notifyIcon;
    private static NotifyViewModel? NotifyViewModel { get; set; }
    public static Window? MainWindow { get; private set; }

    public void UseNotifyIcon()
    {
        NotifyIconInitialize();
    }

    public void NotifyIconInitialize()
    {
        if (NotifyViewModel is not null)
            return;
        NotifyViewModel = XPrismIoc.Fetch<NotifyViewModel>();
        Binding binding = new Binding();
        binding.Source = NotifyViewModel;
        binding.Path = new PropertyPath("Title");
        binding.Mode = BindingMode.TwoWay;
        _notifyIcon = new TaskbarIcon
        {
            DataContext = NotifyViewModel,
            Icon = new System.Drawing.Icon("Assets/logo/logo_32x32.ico"),
            ContextMenu = new ContextMenu()
            {
                Style = (Style)FindResource("Notify")!
            },
        };
        _notifyIcon.SetBinding(TaskbarIcon.ToolTipTextProperty, binding);
        _notifyIcon.DoubleClickCommand = new RelayCommand(MainShow);
    }

    public static void MainShow()
    {
        if (MainWindow is not null)
        {
            MainWindow.Visibility = Visibility.Visible;
            MainWindow.WindowState = WindowState.Normal;
            MainWindow.Activate();
        }
        else
        {
            var windows = Application.Current.Windows;
            foreach (Window item in windows)
            {
                if (item.GetType() != typeof(MainWindow)) continue;
                MainWindow = item;
                item.Visibility = Visibility.Visible;
                item.WindowState = WindowState.Normal;
                item.Activate();
                return;
            }
        }
    }

    public static void DisposeNotifyIcon()
    {
        _notifyIcon?.Dispose();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _notifyIcon?.Dispose();
        base.OnExit(e);
    }

    private void OpenIcon(object recipient, UseIcon message)
    {
        UseNotifyIcon();
        WeakReferenceMessenger.Default.Unregister<UseIcon>(this);
    }
}