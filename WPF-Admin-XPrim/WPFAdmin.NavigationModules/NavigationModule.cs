using WPF.Admin.Models.Models;
using WPF.Admin.Themes.Converter;
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

    public NavigationModule() {
        LoginAuthHelper.ViewAuthSwitch = ViewAuthSwitch;
    }

    public void RegisterTypes(IContainerRegistry containerRegistry) {
        containerRegistry
            .RegisterSingleton<INavigationService, NavigationService>();
        containerRegistry.AddNavigations(regionManager =>
        {
            regionManager.RegisterForNavigation<MainPage, MainViewModel>(RegionName.MainRegion, "Main");
            regionManager.RegisterViewWithRegion<BasePage>(RegionName.HomeRegion, "BasePage");
        });
    }

    public void OnInitialized(IContainerProvider containerProvider) {
    }
}