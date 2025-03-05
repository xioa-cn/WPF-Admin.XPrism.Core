using DataManagerModules.ViewModels;
using DataManagerModules.Views;
using WPF.Admin.Models.Models;
using XPrism.Core.DI;
using XPrism.Core.Modules;
using XPrism.Core.Modules.Find;
using XPrism.Core.Navigations;

namespace DataManagerModules;

[Module(nameof(DataManagerModule))]
public class DataManagerModule:IModule {
    public void RegisterTypes(IContainerRegistry containerRegistry) {
        var regionManager = containerRegistry.Resolve<IRegionManager>();
        regionManager.RegisterForNavigation<DataSearchView, DataSearchViewModel>(RegionName.HomeRegion,
            "DataSearchView");
        regionManager.RegisterForNavigation<ExcelPage, ExcelPageViewModel>(RegionName.HomeRegion,
            "ExcelPage");
        regionManager.RegisterForNavigation<DataSkipView,DataSkipViewModel>(RegionName.HomeRegion,
            "DataSkipView");
    }

    public void OnInitialized(IContainerProvider containerProvider) {
        
    }
}