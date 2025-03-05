using PersonalityComponentModules.ViewModels;
using PersonalityComponentModules.Views;
using WPF.Admin.Models.Models;
using XPrism.Core.DI;
using XPrism.Core.Modules;
using XPrism.Core.Modules.Find;
using XPrism.Core.Navigations;

namespace PersonalityComponentModules;

[Module(nameof(PersonalityComponentModule))]
public class PersonalityComponentModule : IModule {
    public void RegisterTypes(IContainerRegistry containerRegistry) {
        var regionManager = containerRegistry.Resolve<IRegionManager>();
        regionManager.RegisterForNavigation<HeaderView, HeaderViewModel>(RegionName.HomeRegion,
            "HeaderView");
        regionManager.RegisterForNavigation<ParticleClock, ParticleClockViewModel>(RegionName.HomeRegion,
            "ParticleClock");
        regionManager.RegisterForNavigation<PinYinView, PinYinViewModel>(RegionName.HomeRegion,
            "PinYinView");
    }

    public void OnInitialized(IContainerProvider containerProvider) {
    }
}