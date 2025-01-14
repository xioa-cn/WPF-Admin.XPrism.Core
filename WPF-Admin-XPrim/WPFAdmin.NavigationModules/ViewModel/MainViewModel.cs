using System.Collections.ObjectModel;
using WPF.Admin.Models;
using WPF.Admin.Models.Models;
using WPF.Admin.Themes.Converter;
using XPrism.Core.Navigations;

namespace WPFAdmin.NavigationModules.ViewModel;

public partial class MainViewModel : BindableBase {
    public ObservableCollection<TreeItemModel>? TreeItems { get; set; }
    public INavigationService NavigationService { get; set; }

    public MainViewModel(INavigationService navigationService) {
        NavigationService = navigationService;
        RefreshTree();
        
    }

    private async void RefreshTree() {
        this.TreeItems = new ObservableCollection<TreeItemModel>();
        var file = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "router.json");
        var read = System.IO.File.ReadAllText(file);
        var data = System.Text.Json.JsonSerializer.Deserialize<Router>(read);
        if (data?.Routers == null) return;
        foreach (var item in data.Routers)
        {
            this.TreeItems.Add(item);
        }
        await NavigationService.NavigateAsync($"{RegionName.HomeRegion}/BasePage");
    }


    private static LoginUser? _loginUser = null;

    public static LoginUser? LoginUser {
        get => _loginUser;
        set
        {
            AuthChangeView();
            _loginUser = value;
            LoginAuthHelper.LoginUser = value;
        }
    }

    private static void AuthChangeView() {
    }
}