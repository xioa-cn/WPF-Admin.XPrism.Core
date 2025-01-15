using GetColorModules.ViewModels;
using GetColorModules.Views;
using WPF.Admin.Models.Models;
using XPrism.Core.DI;
using XPrism.Core.Modules;
using XPrism.Core.Modules.Find;
using XPrism.Core.Navigations;

namespace GetColorModules;

[Module(nameof(GetColorModule))]
public class GetColorModule :IModule{
    public void RegisterTypes(IContainerRegistry containerRegistry) {
        var regionManager = containerRegistry.Resolve<IRegionManager>();
        regionManager.RegisterForNavigation<GetColorPage, GetColorViewModel>(RegionName.HomeRegion,
            "GetColorPage");
    }

    public void OnInitialized(IContainerProvider containerProvider) {
        
    }
}