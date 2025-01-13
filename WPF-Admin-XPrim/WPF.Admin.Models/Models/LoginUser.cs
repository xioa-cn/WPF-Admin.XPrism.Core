namespace WPF.Admin.Models.Models;

public class LoginUser {
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public string? Header { get; set; }
    public LoginAuth LoginAuth { get; set; }
}