using CommunityToolkit.Mvvm.ComponentModel;

namespace WPF.Admin.Service.Services.Login;

public partial class Tokens : ObservableObject
{
    public static Tokens Instance = new Tokens();
    [ObservableProperty]
    public string? _AccessToken;
    [ObservableProperty]
    public string? _RefreshToken;
}