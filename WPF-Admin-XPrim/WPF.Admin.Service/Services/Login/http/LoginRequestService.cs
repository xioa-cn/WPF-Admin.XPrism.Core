namespace WPF.Admin.Service.Services.Login.http;

public static class LoginRequestService
{
    public class LoginDao
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }

    public static async Task<bool> Login(string userName = "user", string password = "password")
    {
        try
        {
            var response = await ApplicationAxios.Axios.PostAsync<LoginDao>("/Authentication/token", new
            {
                UserName = userName,
                Password = password
            });

            if (response != null)
            {
                await Task.Run(() => RefreshTokenMethod(response));
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            
            return false;
        }
    }

    public static void RefreshTokenMethod(LoginDao token)
    {
        if (token == null) return;


        Tokens.Instance.AccessToken = token.AccessToken;
        Tokens.Instance.RefreshToken = token.RefreshToken;

    }
}