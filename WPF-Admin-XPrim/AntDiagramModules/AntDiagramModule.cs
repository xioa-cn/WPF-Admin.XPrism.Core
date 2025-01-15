using AntDiagramModules.ViewModels;
using AntDiagramModules.Views;
using WPF.Admin.Models.Models;
using XPrism.Core.DI;
using XPrism.Core.Modules;
using XPrism.Core.Modules.Find;
using XPrism.Core.Navigations;

namespace AntDiagramModules;

[Module(nameof(AntDiagramModule))]
public class AntDiagramModule:IModule {
    public void RegisterTypes(IContainerRegistry containerRegistry) {
        var regionManager = containerRegistry.Resolve<IRegionManager>();
        regionManager.RegisterForNavigation<AntDiagramView, AntDiagramViewModel>(RegionName.HomeRegion,
            "AntDiagramView");
    }

    public void OnInitialized(IContainerProvider containerProvider) {
        
    }
}