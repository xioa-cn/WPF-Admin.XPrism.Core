using WPF.Admin.Models.Models;
using WPFAdmin.NavigationModules.Components;
using WPFAdmin.NavigationModules.ViewModel;
using WPFAdmin.NavigationModules.Views;
using XPrism.Core.DI;
using XPrism.Core.Modules;
using XPrism.Core.Modules.Find;
using XPrism.Core.Navigations;

namespace WPFAdmin.NavigationModules;

[Module(nameof(NavigationModule))]
public class NavigationModule : IModule {
    public static ViewAuthSwitch ViewAuthSwitch { get; set; } = ViewAuthSwitch.IsEnabled;

    public void RegisterTypes(IContainerRegistry containerRegistry) {
        containerRegistry
            .RegisterSingleton<INavigationService, NavigationService>();
        containerRegistry.AddNavigations(regionManager =>
        {
            regionManager.RegisterForNavigation<MainPage, MainViewModel>("MainRegion", "Main");
            regionManager.RegisterViewWithRegion<BasePage>("Home", "BasePage");
        });
    }

    public void OnInitialized(IContainerProvider containerProvider) {
    }
}