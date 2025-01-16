using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HandyControl.Controls;
using WPF.Admin.Models;
using WPF.Admin.Models.Models;
using WPF.Admin.Service.Services.Login.http;
using WPF.Admin.Themes.Converter;

namespace WPFAdmin.LoginModules;

public partial class LoginViewModel : BindableBase {
    [ObservableProperty] private string? _userName;
    [ObservableProperty] private string? _password;
    [ObservableProperty] private bool _rememberPassword;
    public LoginViewModel()
    {
        UserName = "xioa";
    }
    [RelayCommand]
    private async Task Login(System.Windows.Window window)
    {
        window.IsEnabled = false;

        try
        {
            var result = await LoginRequestService.Login();

            if (result || true)
            {
                LoginAuthHelper.LoginUser = new LoginUser() {
                    UserName = UserName,
                    Password = Password,
                    LoginAuth = LoginAuth.Admin,
                };
                window.Close();
                Growl.Success($"Login Success!! {UserName}");
            }
        }
        catch (System.Exception ex)
        {
            MessageBox.Show(ex.Message);

        }
        window.IsEnabled = true;
    }
}