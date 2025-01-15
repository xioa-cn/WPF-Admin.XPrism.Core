using TopicModules.ViewModels;
using TopicModules.Views;
using WPF.Admin.Models.Models;
using XPrism.Core.DI;
using XPrism.Core.Modules;
using XPrism.Core.Modules.Find;
using XPrism.Core.Navigations;

namespace TopicModules;

[Module(nameof(ColorSystemModule))]
public class ColorSystemModule:IModule {
    public void RegisterTypes(IContainerRegistry containerRegistry) {
        var regionManager = containerRegistry.Resolve<IRegionManager>();
        regionManager.RegisterForNavigation<ColorSystemPage, ColorSystemViewModel>(RegionName.HomeRegion,
            "ColorSystemPage");
    }

    public void OnInitialized(IContainerProvider containerProvider) {
        
    }
}