
using WPF.Xlog.Logger.Impl;
using WPF.Xlog.Logger.Service;


namespace WPF.Admin.Service.Logger
{
    public class XLogGlobal
    {
        public static ILogService? Logger => LogService.Instance;
        static XLogGlobal()
        {
            LogService.CreateLoggerInstance(maxFileSizeInMB: 1000, maxLogFiles: 10);
        }
    }
}
