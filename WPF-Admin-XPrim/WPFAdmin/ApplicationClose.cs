using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFAdmin
{
    public partial class App
    {
        public static void DisposeAppResources()
        {
            // 清理资源
            //TrackingManager.Instance.Dispose();
            App.DisposeNotifyIcon();
        }
    }
}
