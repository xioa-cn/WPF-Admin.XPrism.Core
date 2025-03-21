using CommunityToolkit.Mvvm.Messaging;
using System.Windows.Forms;
using WPF.Admin.Models.Models;
using WPF.Admin.Themes.Themes;
using WPFAdmin.Config;
using WPFAdmin.LoginModules;
using XPrism.Core.DI;

namespace WPFAdmin;

public partial class App
{
    private void StartupWindow(Views.SplashScreen splashScreen)
    {
        var s = Enum.TryParse<IndexStatus>(Configs.Default?.IndexStatus, out var indexStatus);

        if (!s)
        {
            MessageBox.Show("初始化窗口异常！！！", "Error");
            Environment.Exit(0);
            return;
        }

        switch (indexStatus)
        {
            case IndexStatus.Login:
                {
                    var login = XPrismIoc.Fetch<LoginWindow>();
                    splashScreen.SwitchWindow(login);
                    WeakReferenceMessenger.Default.Register<UseIcon>(this, OpenIcon);
                    break;
                }
            case IndexStatus.Main:
                {
                    var mainWindow =
                        XPrismIoc.FetchXPrismWindow(nameof(MainWindow));
                    
                    // 直接进入main窗口时 查看上次登录的权限
                    AuthLoaded();
                    splashScreen.SwitchWindow(mainWindow);
                    NotifyIconInitialize();
                    break;
                }
            default:
                throw new ArgumentOutOfRangeException();
        }
        ElMessage.Wpf.Utils.ElMessage.Success("Welcome");
    }

    private void AuthLoaded() {
        // 在登录记录中查看
    }
}