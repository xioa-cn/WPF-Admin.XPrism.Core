using TopicModules.ViewModels;
using TopicModules.Views;
using WPF.Admin.Models.Models;
using XPrism.Core.DI;
using XPrism.Core.Modules;
using XPrism.Core.Modules.Find;
using XPrism.Core.Navigations;

namespace TopicModules;

[Module(nameof(BlendentModule))]
public class BlendentModule:IModule {
    public void RegisterTypes(IContainerRegistry containerRegistry) {
        var regionManager = containerRegistry.Resolve<IRegionManager>();
        regionManager.RegisterForNavigation<BlendentPage, BlendentViewModel>(RegionName.HomeRegion,
            "BlendentPage");
    }

    public void OnInitialized(IContainerProvider containerProvider) {
        
    }
}