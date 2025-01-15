using CarouselModules.ViewModels;
using CarouselModules.Views;
using WPF.Admin.Models.Models;
using XPrism.Core.DI;
using XPrism.Core.Modules;
using XPrism.Core.Modules.Find;
using XPrism.Core.Navigations;

namespace CarouselModules;

[Module(nameof(CarouselModule))]
public class CarouselModule:IModule {
    public void RegisterTypes(IContainerRegistry containerRegistry) {
        var regionManager = containerRegistry.Resolve<IRegionManager>();
        regionManager.RegisterForNavigation<CarouselView, CarouselViewModel>(RegionName.HomeRegion,
            "CarouselView");
    }

    public void OnInitialized(IContainerProvider containerProvider) {
       
    }
}