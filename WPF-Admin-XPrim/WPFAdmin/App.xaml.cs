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
        SystemTheme(); // 系统主题
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
        Detect(); // 检测是否为多开
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
                // 注册登录窗口
                ContainerLocator.Container.RegisterTransient<LoginWindow>();
                ContainerLocator.Container.RegisterEventAggregator<EventAggregator>(); // 注册事件总线
                ContainerLocator.Container.AutoRegisterByAttribute(Assembly.Load("WPFAdmin")); // 注册WPFAdmin中AutoRegisterAttribute特性类
                ContainerLocator.Container
                    .RegisterSingleton<IModuleFinder>(new DirectoryModuleFinder()) // 添加基于目录的模块发现器
                    .RegisterMeModuleManager(manager => { manager.LoadModulesConfig(AppDomain.CurrentDomain.BaseDirectory); }); // 指定路径
                ContainerLocator.Container.AutoRegisterByAttribute<XPrismViewModelAttribute>(
                    Assembly.Load("WPFAdmin"));// 注册WPFAdmin中XPrismViewModelAttribute特性类
                ContainerLocator.Container.Build();

                var applicationStartupMode = StartupCommandLine(e.Args); // 接收命令行参数

                if (applicationStartupMode == ApplicationStartupMode.Debug) // Debug模式不需要执行之后的代码
                {
                    Environment.Exit(0);
                }

                // 启动主窗口
                StartupWindow(splashScreen);
            });
        });

        XLogGlobal.Logger?.LogInfo("打开了软件");
    }
}