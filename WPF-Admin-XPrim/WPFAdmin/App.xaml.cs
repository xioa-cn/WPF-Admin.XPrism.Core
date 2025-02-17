using System.Reflection;
using System.Windows;
using WPF.Admin.Models.EFDbContext.Temp;
using WPF.Admin.Models.Models;
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
        Detect();
        base.OnStartup(e);

       
        Views.SplashScreen splashScreen = new Views.SplashScreen();
        splashScreen.ShowWindowWithFade();

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
    }
}