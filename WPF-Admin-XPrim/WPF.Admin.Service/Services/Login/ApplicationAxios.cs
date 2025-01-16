using Xioa.Admin.Request.Tools.NetAxios;

namespace WPF.Admin.Service.Services.Login;

public static class ApplicationAxios
{
    private static IAxios? _axios;

    public static IAxios Axios
    {
        get
            => _axios ??= new NAxios(AxiosConfig, IgnoreSslErrorsSslError);
    }

    private static NAxiosConfig? _axiosConfig;

    private static NAxiosConfig? AxiosConfig
    {
        get
        {
            if (_axiosConfig is null)
                throw new NullReferenceException();
            return _axiosConfig;
        }
    }

    private static bool IgnoreSslErrorsSslError { get; set; }

    public static void SetAxiosConfig(NAxiosConfig? nAxiosConfig, bool sslError = false)
    {
        IgnoreSslErrorsSslError = sslError;
        _axiosConfig = nAxiosConfig;
    }
}