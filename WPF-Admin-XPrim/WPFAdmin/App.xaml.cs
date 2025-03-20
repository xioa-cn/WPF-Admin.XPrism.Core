using System.Diagnostics;
using System.Reflection;
using System.Windows;
using WPF.Admin.Models.Models;
using WPF.Admin.Service.Logger;
using WPF.Admin.Service.Services;
using WPF.Admin.Service.Services.Login;
using WPF.Admin.Themes;
using WPF.Admin.Themes.Helper;
using WPF.Admin.Themes.Themes;
using WPFAdmin.LoginModules;
using XPrism.Core.DataContextWindow;
using XPrism.Core.DI;
using XPrism.Core.Events;
using XPrism.Core.Modules.Find;

namespace WPFAdmin;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application {
    protected override void OnStartup(StartupEventArgs e) {
        SystemTheme();
        // 设置全局异常处理
        AppDomain.CurrentDomain.UnhandledException += (s, args) =>
        {
            XLogGlobal.Logger?.LogFatal("Unhandled Exception", args.ExceptionObject as Exception);
        };

        Current.DispatcherUnhandledException += (s, args) =>
        {
            XLogGlobal.Logger?.LogError("Dispatcher Unhandled Exception", args.Exception);
            //args.Handled = true;
        };
        Detect();
        base.OnStartup(e);
        DispatcherHelper.Initialize();
        
        Views.SplashScreen splashScreen = new Views.SplashScreen();
        splashScreen.ShowWindowWithFade();
        Task.Run(() =>
        {
            ApplicationAxiosConfig.Initialized(); // 初始化网络请求A
            Thread.Sleep(1000);
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                ContainerLocator.Container.RegisterSingleton<LoginWindow>();
                ContainerLocator.Container.RegisterEventAggregator<EventAggregator>();
                ContainerLocator.Container.AutoRegisterByAttribute(Assembly.Load("WPFAdmin"));
                ContainerLocator.Container
                    .RegisterSingleton<IModuleFinder>(new DirectoryModuleFinder())
                    .RegisterMeModuleManager(manager => { manager.LoadModulesConfig(AppDomain.CurrentDomain.BaseDirectory); });
                ContainerLocator.Container.AutoRegisterByAttribute<XPrismViewModelAttribute>(
                    Assembly.Load("WPFAdmin"));
                ContainerLocator.Container.Build();

                var applicationStartupMode = StartupCommandLine(e.Args);

                if (applicationStartupMode == ApplicationStartupMode.Debug)
                {
                    Environment.Exit(0);
                }

                StartupWindow(splashScreen);
            });
        });

        XLogGlobal.Logger?.LogInfo("打开了软件");
    }
}