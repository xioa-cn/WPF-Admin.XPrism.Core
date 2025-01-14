using GanttChartModules.ViewModels;
using GanttChartModules.Views;
using WPF.Admin.Models.Models;
using XPrism.Core.DI;
using XPrism.Core.Modules;
using XPrism.Core.Modules.Find;
using XPrism.Core.Navigations;

namespace GanttChartModules;

[Module(nameof(GanttChartModule))]
public class GanttChartModule : IModule {
    public void RegisterTypes(IContainerRegistry containerRegistry) {
        var regionManager = containerRegistry.Resolve<IRegionManager>();
        regionManager.RegisterForNavigation<GanttChartPage, GanttChartViewModel>(RegionName.HomeRegion,
            "GanttChartPage");
      
    }

    public void OnInitialized(IContainerProvider containerProvider) {
    }
}