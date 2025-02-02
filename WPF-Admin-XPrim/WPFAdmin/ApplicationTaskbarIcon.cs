﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using CommunityToolkit.Mvvm.Input;
using Hardcodet.Wpf.TaskbarNotification;
using WPFAdmin.ViewModels;
using WPFAdmin.Views;
using XPrism.Core.DI;

namespace WPFAdmin;

public partial class App {
    private static TaskbarIcon? _notifyIcon;
    private static NotifyViewModel? NotifyViewModel { get; set; }

    private void NotifyIconInitialize() {
        if (NotifyViewModel is not null)
            return;
        NotifyViewModel = XPrismIoc.Fetch<NotifyViewModel>();
        Binding binding = new Binding();
        binding.Source = NotifyViewModel;
        binding.Path = new PropertyPath("Title");
        binding.Mode = BindingMode.TwoWay;
        _notifyIcon = new TaskbarIcon {
            DataContext = NotifyViewModel,
            Icon = new System.Drawing.Icon("Assets/logo/logo_32x32.ico"),
            ContextMenu = new ContextMenu() {
                Style = (Style)FindResource("Notify")!
            },
        };
        _notifyIcon.SetBinding(TaskbarIcon.ToolTipTextProperty, binding);
        _notifyIcon.DoubleClickCommand = new RelayCommand(MainShow);
    }

    public static void MainShow() {
        if (Application.Current.MainWindow is not null)
            Application.Current.MainWindow.Visibility = Visibility.Visible;
        else
        {
            var windows = Application.Current.Windows;
            foreach (Window item in windows)
            {
                if (item.GetType() != typeof(MainWindow)) continue;
                item.Visibility = Visibility.Visible;
                return;
            }
        }
    }

    public static void DisposeNotifyIcon() {
        _notifyIcon?.Dispose();
    }

    protected override void OnExit(ExitEventArgs e) {
        _notifyIcon?.Dispose();
        base.OnExit(e);
    }
}