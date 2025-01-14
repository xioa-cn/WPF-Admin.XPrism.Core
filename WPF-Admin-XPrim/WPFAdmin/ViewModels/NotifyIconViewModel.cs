using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using HandyControl.Tools.Extension;
using WPF.Admin.Models;
using WPF.Admin.Models.Models;
using WPFAdmin.Views;

namespace WPFAdmin.ViewModels;

public partial class NotifyIconViewModel : BindableBase, IDialogResultable<CloseEnum> {
    public CloseEnum Result { get; set; }
    public Action CloseAction { get; set; }


    [ObservableProperty] private bool _close;
    [ObservableProperty] private bool _mini = true;

    [RelayCommand]
    private void Closed() {
        if (Close)
        {
            this.Result = CloseEnum.Close;
        }
        else if (Mini)
        {
            this.Result = CloseEnum.Notify;
        }

        Dialog.Close(HcDialogMessageToken.DialogMainToken);
    }

    [RelayCommand]
    private void Cancel() {
        this.Result = CloseEnum.None;
        Dialog.Close(HcDialogMessageToken.DialogMainToken);
    }
}