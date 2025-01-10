using WPFAdmin.NavigationModules.ViewModel;
using WPFAdmin.NavigationModules.Views;
using XPrism.Core.DI;
using XPrism.Core.Modules;
using XPrism.Core.Modules.Find;
using XPrism.Core.Navigations;

namespace WPFAdmin.NavigationModules;

[Module(nameof(NavigationModule))]
public class NavigationModule : IModule {
    public void RegisterTypes(IContainerRegistry containerRegistry) {
        containerRegistry
            .RegisterSingleton<INavigationService, NavigationService>();
        containerRegistry.AddNavigations(regionManager =>
        {
            regionManager.RegisterForNavigation<MainPage,MainViewModel>("MainRegion","Main");
        });
    }

    public void OnInitialized(IContainerProvider containerProvider) {
       
    }
}