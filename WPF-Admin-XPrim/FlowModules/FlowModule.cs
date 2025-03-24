using FlowModules.ViewModels;
using FlowModules.Views;
using WPF.Admin.Models.Models;
using XPrism.Core.DI;
using XPrism.Core.Modules;
using XPrism.Core.Modules.Find;
using XPrism.Core.Navigations;

namespace FlowModules;

[Module(nameof(FlowModule))]
public class FlowModule : IModule {
    public void RegisterTypes(IContainerRegistry containerRegistry) {
        var regionManager = containerRegistry.Resolve<IRegionManager>();
        regionManager.RegisterForNavigation<FlowPage, FlowViewModel>(RegionName.HomeRegion,
            "FlowPage");
    }

    public void OnInitialized(IContainerProvider containerProvider) {
    }
}