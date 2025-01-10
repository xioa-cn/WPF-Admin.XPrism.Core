using XPrism.Core.DI;
using XPrism.Core.Modules;
using XPrism.Core.Modules.Find;

namespace WPFAdmin.DialogModules;

[Module(nameof(DialogModule))]
public class DialogModule : IModule {
    public void RegisterTypes(IContainerRegistry containerRegistry) {
        containerRegistry.RegisterDialogServiceCommonBase();
    }

    public void OnInitialized(IContainerProvider containerProvider) {
        
    }
}