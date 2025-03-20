using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using WPF.Admin.Models;
using WPF.Admin.Models.Models;
using WPF.Admin.Service.Services.Login.http;
using WPF.Admin.Themes.Converter;

namespace WPFAdmin.LoginModules;

public partial class LoginViewModel : BindableBase {
    [ObservableProperty] private string? _password;
    [ObservableProperty] private bool _rememberPassword;
    [ObservableProperty] private string? _InputText;


    public ObservableCollection<string> Items { get; set; } = new ObservableCollection<string>() {
        "xioa","admin","test"
    };

    [RelayCommand]
    private void Delete(string value) {
        Items.Remove(value);
    }

    public LoginViewModel() {
        InputText = "xioa";
    }

    [RelayCommand]
    private async Task Login(System.Windows.Window window) {
        window.IsEnabled = false;

        try
        {
            var result = await LoginRequestService.Login();

            if (result || true)
            {
                LoginAuthHelper.LoginUser = new LoginUser() {
                    UserName = InputText,
                    Password = Password,
                    LoginAuth = LoginAuth.Admin,
                };
                (window as LoginWindow).SuccessLogin();
                Growl.Success($"Login Success!! {InputText}");
            }
        }
        catch (System.Exception ex)
        {
            MessageBox.Show(ex.Message);
        }

        window.IsEnabled = true;
    }
}