using SharedMemoryDemoModules.ViewModels;
using SharedMemoryDemoModules.Views;
using WPF.Admin.Models.Models;
using XPrism.Core.DI;
using XPrism.Core.Modules;
using XPrism.Core.Modules.Find;
using XPrism.Core.Navigations;

namespace SharedMemoryDemoModules;

[Module(nameof(SharedMemoryModule))]
public class SharedMemoryModule:IModule{
    public void RegisterTypes(IContainerRegistry containerRegistry) {
        var regionManager = containerRegistry.Resolve<IRegionManager>();
        regionManager.RegisterForNavigation<SharedMemoryPage, SharedMemoryViewModel>(RegionName.HomeRegion,
            "SharedMemoryPage");
    }

    public void OnInitialized(IContainerProvider containerProvider) {
        
    }
}