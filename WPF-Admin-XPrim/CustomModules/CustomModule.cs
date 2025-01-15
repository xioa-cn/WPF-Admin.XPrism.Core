using CustomModules.ViewModels;
using CustomModules.Views;
using WPF.Admin.Models.Models;
using XPrism.Core.DI;
using XPrism.Core.Events;
using XPrism.Core.Modules;
using XPrism.Core.Modules.Find;
using XPrism.Core.Navigations;

namespace CustomModules;

[Module(nameof(CustomModule))]
public class CustomModule(IEventAggregator eventAggregator) : IModule {
    private readonly IEventAggregator _eventAggregator = eventAggregator;

    public void RegisterTypes(IContainerRegistry containerRegistry) {
        var regionManager = containerRegistry.Resolve<IRegionManager>();
        regionManager.RegisterForNavigation<UseQQComboBox, QQComboBoxViewModel>(RegionName.HomeRegion,
            "UseQQComboBox");
        regionManager.RegisterForNavigation<VirtualizingListPage, VirtualizingListViewModel>(RegionName.HomeRegion,
            "VirtualizingListPage");
    }

    public void OnInitialized(IContainerProvider containerProvider) {
       
    }
}