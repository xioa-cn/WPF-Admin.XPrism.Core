using DataValidatorModules.ViewModels;
using DataValidatorModules.Views;
using WPF.Admin.Models.Models;
using XPrism.Core.DI;
using XPrism.Core.Modules;
using XPrism.Core.Modules.Find;
using XPrism.Core.Navigations;

namespace DataValidatorModules;

[Module(nameof(DataValidatorModule))]
public class DataValidatorModule:IModule {
    public void RegisterTypes(IContainerRegistry containerRegistry) {
        var regionManager = containerRegistry.Resolve<IRegionManager>();
        regionManager.RegisterForNavigation<DataValidatorPage, ValidatorViewModel>(RegionName.HomeRegion,
            "DataValidatorPage");
    }

    public void OnInitialized(IContainerProvider containerProvider) {
        
    }
}