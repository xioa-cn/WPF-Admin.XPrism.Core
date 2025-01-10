using System.Windows.Controls;
using XPrism.Core.Navigations;

namespace WPFAdmin.NavigationModules.Views;

public partial class MainPage : Page,INavigationAware {
    public MainPage() {
        InitializeComponent();
    }

    public async Task OnNavigatingToAsync(INavigationParameters parameters) {
        
    }

    public async Task OnNavigatingFromAsync(INavigationParameters parameters) {
       
    }

    public async Task<bool> CanNavigateToAsync(INavigationParameters parameters) {
        return true;
    }

    public async Task<bool> CanNavigateFromAsync(INavigationParameters parameters) {
        return true;
    }
}