using System.Reflection;
using System.Windows;
using WPF.Admin.Themes.Themes;
using XPrism.Core.DataContextWindow;
using XPrism.Core.DI;
using XPrism.Core.Events;
using XPrism.Core.Modules.Find;
using MessageBox = HandyControl.Controls.MessageBox;

namespace WPFAdmin;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application {
    protected override void OnStartup(StartupEventArgs e) {
        base.OnStartup(e);
        Views.SplashScreen splashScreen = new Views.SplashScreen();
        StartupCommandLine(e.Args);
        splashScreen.ShowWindowWithFade();
        if (Detection)
        {
            MessageBox.Show("本程序一次只能运行一个实例！", "提示");
            Application.Current.Shutdown();
            Environment.Exit(0);
            return;
        }

        ContainerLocator.Container.RegisterEventAggregator<EventAggregator>();
        ContainerLocator.Container.AutoRegisterByAttribute(Assembly.Load("WPFAdmin"));
        ContainerLocator.Container
            .RegisterSingleton<IModuleFinder>(new DirectoryModuleFinder())
            .RegisterMeModuleManager(manager => { manager.LoadModulesConfig(AppDomain.CurrentDomain.BaseDirectory); });
        ContainerLocator.Container.AutoRegisterByAttribute<XPrismViewModelAttribute>(
            Assembly.Load("WPFAdmin"));
        ContainerLocator.Container.Build();
        var mainWindow =
            XPrismIoc.FetchXPrismWindow(nameof(MainWindow));
        NotifyIconInitialize();
        splashScreen.SwitchWindow(mainWindow);
    }
}