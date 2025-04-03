using PictureModules.ViewModels;
using PictureModules.Views;
using WPF.Admin.Models.Models;
using XPrism.Core.DI;
using XPrism.Core.Modules;
using XPrism.Core.Modules.Find;
using XPrism.Core.Navigations;

namespace PictureModules;

[Module(nameof(PictureModule))]
public class PictureModule : IModule
{
    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        var regionManager = containerRegistry.Resolve<IRegionManager>();
        regionManager.RegisterForNavigation<PictureView, PictureViewModel>(RegionName.HomeRegion,
            "PictureView");
    }

    public void OnInitialized(IContainerProvider containerProvider)
    {
    }
}