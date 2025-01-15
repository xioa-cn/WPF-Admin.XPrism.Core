using QrCodeModules.ViewModels;
using QrCodeModules.Views;
using WPF.Admin.Models.Models;
using XPrism.Core.DI;
using XPrism.Core.Modules;
using XPrism.Core.Modules.Find;
using XPrism.Core.Navigations;

namespace QrCodeModules;

[Module(nameof(QrCodeModule))]
public class QrCodeModule: IModule {
    public void RegisterTypes(IContainerRegistry containerRegistry) {
        var regionManager = containerRegistry.Resolve<IRegionManager>();
        regionManager.RegisterForNavigation<QrCodeView, QrCodeViewModel>(RegionName.HomeRegion,
            "QrCodeView");
    }

    public void OnInitialized(IContainerProvider containerProvider) {
        
    }
}