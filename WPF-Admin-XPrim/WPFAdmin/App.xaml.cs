using System.Reflection;
using System.Windows;
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
        base.OnStartup(e);
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
        mainWindow.ShowDialog();
    }
}