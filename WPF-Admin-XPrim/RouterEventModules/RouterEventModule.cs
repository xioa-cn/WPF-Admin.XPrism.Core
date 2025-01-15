using RouterEventModules.ViewModels;
using RouterEventModules.Views;
using WPF.Admin.Models.Models;
using XPrism.Core.DI;
using XPrism.Core.Modules;
using XPrism.Core.Modules.Find;
using XPrism.Core.Navigations;

namespace RouterEventModules;

[Module(nameof(RouterEventModule))]
public class RouterEventModule : IModule {
    public void RegisterTypes(IContainerRegistry containerRegistry) {
        var regionManager = containerRegistry.Resolve<IRegionManager>();
        regionManager.RegisterForNavigation<RouterEventPage, RouterEventViewModel>(RegionName.HomeRegion,
            "RouterEventPage");
    }

    public void OnInitialized(IContainerProvider containerProvider) {
    }
}