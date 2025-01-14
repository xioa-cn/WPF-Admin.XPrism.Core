using AboutModules.Views;
using WPF.Admin.Models.Models;
using XPrism.Core.DI;
using XPrism.Core.Modules;
using XPrism.Core.Modules.Find;
using XPrism.Core.Navigations;

namespace AboutModules;

[Module(nameof(AboutModule))]
public class AboutModule : IModule {
    public void RegisterTypes(IContainerRegistry containerRegistry) {
        var regionManager = containerRegistry.Resolve<IRegionManager>();
        regionManager.RegisterViewWithRegion<AboutView>(RegionName.HomeRegion, "AboutPage");
        
    }

    public void OnInitialized(IContainerProvider containerProvider) {
    }
}