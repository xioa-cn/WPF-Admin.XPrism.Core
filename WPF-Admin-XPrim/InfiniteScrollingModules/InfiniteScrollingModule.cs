using InfiniteScrollingModules.ViewModel;
using InfiniteScrollingModules.Views;
using WPF.Admin.Models.Models;
using XPrism.Core.DI;
using XPrism.Core.Modules;
using XPrism.Core.Modules.Find;
using XPrism.Core.Navigations;

namespace InfiniteScrollingModules;

[Module(nameof(InfiniteScrollingModule))]
public class InfiniteScrollingModule : IModule {
    public void RegisterTypes(IContainerRegistry containerRegistry) {
        var regionManager = containerRegistry.Resolve<IRegionManager>();
        regionManager.RegisterForNavigation<InfiniteScrollingView, InfiniteScrollingViewModel>(RegionName.HomeRegion,
            "InfiniteScrollingView");
    }

    public void OnInitialized(IContainerProvider containerProvider) {
    }
}