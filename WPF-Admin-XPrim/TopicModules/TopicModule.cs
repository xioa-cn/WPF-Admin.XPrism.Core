using TopicModules.ViewModels;
using TopicModules.Views;
using WPF.Admin.Models.Models;
using XPrism.Core.DI;
using XPrism.Core.Modules;
using XPrism.Core.Modules.Find;
using XPrism.Core.Navigations;

namespace TopicModules;

[Module(nameof(TopicModule))]
public class TopicModule : IModule {
    public void RegisterTypes(IContainerRegistry containerRegistry) {
        var regionManager = containerRegistry.Resolve<IRegionManager>();
        regionManager.RegisterForNavigation<TopicView, TopicViewModel>(RegionName.HomeRegion,
            "TopicView");
    
    }

    public void OnInitialized(IContainerProvider containerProvider) {
    }
}