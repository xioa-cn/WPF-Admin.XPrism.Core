using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using WPF.Admin.Models;
using XPrism.Core.DI;

namespace WPFAdmin.ViewModels;

[AutoRegister(typeof(NotifyViewModel), ServiceLifetime.Singleton, nameof(NotifyViewModel))]
public partial class NotifyViewModel : BindableBase {
    public NotifyViewModel() {
    }


    #region Title

    [ObservableProperty] private string _title = nameof(WPFAdmin);
    [ObservableProperty] private bool _isCheck = true;

    #endregion

    [RelayCommand]
    private void Open() {
        App.MainShow();
    }

    [RelayCommand]
    private void Close() {
        App.DisposeNotifyIcon();
        Environment.Exit(0);
    }
}