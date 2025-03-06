

using ChargeModules.ViewModels;
using ChargeModules.Views;
using WPF.Admin.Models.Models;
using XPrism.Core.DI;
using XPrism.Core.Modules;
using XPrism.Core.Modules.Find;
using XPrism.Core.Navigations;

namespace ChargeModules
{
    [Module(nameof(ChargeModule))]
    public class ChargeModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            var regionManager = containerRegistry.Resolve<IRegionManager>();
            regionManager.RegisterForNavigation<ChargeView, ChargeViewModel>(RegionName.HomeRegion,
                "ChargeView");
            regionManager.RegisterForNavigation<Charge2View, Charge2ViewModel>(RegionName.HomeRegion,
                "Charge2View");
        }
    }
}
