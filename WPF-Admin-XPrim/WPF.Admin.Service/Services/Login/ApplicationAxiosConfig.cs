using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using Xioa.Admin.Request.Tools.NetAxios;

namespace WPF.Admin.Service.Services.Login;

public static class ApplicationAxiosConfig
{
    public static void Initialized()
    {
        // 初始化 NAxios
        ApplicationAxios.SetAxiosConfig(new NAxiosConfig
        {
            BaseUrl = "http://localhost:7078/api/",
            Timeout = 5000,
            Headers =
            {
                //["Content-Type"]="application/json",
                ["accept"] = "*/*",
            },
            RetryCount = 3,
            RetryDelay = 3000,
            RetryCondition = (exception, retryCount) =>
            {
                var res = exception switch
                {
                    HttpRequestException httpException =>
                        // 5xx 服务器错误
                        httpException.StatusCode >= System.Net.HttpStatusCode.InternalServerError ||
                        // 408 请求超时
                        httpException.StatusCode == System.Net.HttpStatusCode.RequestTimeout ||
                        // 429 太多请求
                        httpException.StatusCode == System.Net.HttpStatusCode.TooManyRequests,
                    TaskCanceledException => true, // 超时
                    SocketException => true, // 网络连接错误
                    IOException => true, // IO错误（通常是网络相关）
                    _ => false // 其他错误不重试
                };
                return res;
            }
        });

        ApplicationAxios.Axios.AddRequestInterceptor(async (request) =>
        {
            if (Tokens.Instance.AccessToken is not null && !request.RequestUri.ToString().Contains("/Authentication/refresh-token"))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Tokens.Instance.AccessToken);
            }

            if (Tokens.Instance.RefreshToken is not null && request.RequestUri.ToString().Contains("/Authentication/refresh-token")
            )
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Tokens.Instance.RefreshToken);
            }

            // 可以修改请求URL
            if (request.RequestUri == null || request.RequestUri.AbsolutePath.StartsWith("/api")) return request;
            var newUri = new Uri(request.RequestUri.AbsoluteUri.Replace(
                request.RequestUri.AbsolutePath,
                "/api" + request.RequestUri.AbsolutePath));
            request.RequestUri = newUri;

            return request;
        });
    }
}