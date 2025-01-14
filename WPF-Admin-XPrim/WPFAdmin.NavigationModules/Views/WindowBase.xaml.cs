using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using CommunityToolkit.Mvvm.Messaging;
using HandyControl.Controls;
using WPF.Admin.Models.Models;
using WPFAdmin.NavigationModules.Components;
using WPFAdmin.NavigationModules.Messengers;
using WPFAdmin.NavigationModules.ViewModel;
using Window = System.Windows.Window;

namespace WPFAdmin.NavigationModules.Views;

public partial class WindowBase : Window {
    public TreeItemModel? ItemModel { get; set; }

    public WindowBase(TreeItemModel model, double width = 800, double height = 450) {
        this.Loaded += Base_Loaded;
        model.PageStatus = PageStatus.Windows;
        ItemModel = model;
        InitializeComponent();
        this.Width = width;
        this.Height = height;
        this.Title = ItemModel.Content;
        //this.frame.Navigate(ItemModel.Page);
        this.Unloaded += (s, e) => { Dialog.Unregister(HcDialogMessageToken.DialogEjectToken, this); };
    }

    public void ShowAndActivated() {
        this.Show();
        this.Activate();
        this.Focusable = true;
    }

    private async void Base_Loaded(object sender, RoutedEventArgs e) {
        Dialog.Register(HcDialogMessageToken.DialogEjectToken, this);
        if (ItemModel is not null)
            await InitializedView(ItemModel);
    }

    private async Task InitializedView(TreeItemModel model) {
        if (this.DataContext is MainViewModel viewModel)
        {
            var element =
                await viewModel.NavigationService.FetchNavigateViewAsync($"{RegionName.HomeRegion}/{model.Page}");

            if (element is System.Windows.Controls.Page value)
            {
                var frame = new Frame();
                frame.Navigate(value);
                ContentControl.Content = frame;
            }
            else if (element is UserControl userControl)
            {
                ContentControl.Content = userControl;
            }
        }
    }


    private void MaxSize_Click(object sender, RoutedEventArgs e) {
        this.WindowState = this.WindowState switch {
            WindowState.Normal => WindowState.Maximized,
            WindowState.Maximized => WindowState.Normal,
            _ => this.WindowState
        };
    }

    private async void Close_Click(object sender, RoutedEventArgs e) {
        if (ItemModel is not null)
            BreadCrumbBar.Items.Remove(ItemModel);
        this.Close();
    }

    private async void BackWindow_Click(object? sender, RoutedEventArgs? e) {
        if (this.DataContext is MainViewModel viewModel)
        {
        }

        if (ItemModel.PageCanInterchange == PageCanInterchange.NonePage)
        {
            return;
        }

        this.Close();
        BreadCrumbBar.Items.Remove(ItemModel);
        ItemModel.IsChecked = true;
        var page = new TreeItemModelMessenger() {
            Item = ItemModel,
            MessengerStatus = MessengerStatus.FromWindowToPage,
        };
        WeakReferenceMessenger.Default.Send(page);
    }

    private void MiniSize_Click(object sender, RoutedEventArgs e) {
        this.WindowState = WindowState.Minimized;
    }
}