using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Messaging;
using HandyControl.Controls;
using WPF.Admin.Models.Models;
using WPFAdmin.NavigationModules.Messengers;
using WPFAdmin.NavigationModules.ViewModel;
using WPFAdmin.NavigationModules.Views;
using MessageBox = HandyControl.Controls.MessageBox;

namespace WPFAdmin.NavigationModules.Components;

public partial class BreadCrumbBar : UserControl {
    public static Dictionary<TreeItemModel, System.Windows.Window> Items { get; set; } =
        new Dictionary<TreeItemModel, System.Windows.Window>();


    private ObservableCollection<TreeItemModel>? _baseItem;

    public ObservableCollection<TreeItemModel>? BaseList {
        get => _baseItem;
        set { _baseItem = value; }
    }

    private async Task<TreeItemModel?> BaselistRemove(TreeItemModel value) {
        if (BaseList == null) return null;
        var index = Array.IndexOf(BaseList.ToArray(), value);
        TreeItemModel? page = null;
        
        if (index > 0 && BaseList[index].IsChecked)
        {
            BaseList[index - 1].IsChecked = true;
            page = BaseList[index - 1];
        }
        else if (index == 0 && BaseList.Count > 1 && BaseList[index].IsChecked)
        {
            BaseList[1].IsChecked = true;
            page = BaseList[1];
        }

        //value.IsChecked = false;
        this.BaseList.Remove(value);
        if (this.BaseList.Count >= 1) return page;
        this.HeaderBorder.Visibility = Visibility.Collapsed;
        if (this.DataContext is MainViewModel vm)
        {
            var result = await vm.NavigationService.NavigateAsync($"{RegionName.HomeRegion}/BasePage");
            if (!result)
            {
            }
        }
        NaviControl.olditemModel = page;
        return page;
    }

    private void BaselistAdd(TreeItemModel value) {
        var treeItemModels = this.BaseList;
        if (treeItemModels != null)
        {
            var r = treeItemModels.FirstOrDefault(x => x == value);
            if (r is not null)
            {
                r.IsChecked = true;
            }
            else
            {
                treeItemModels.Add(value);
            }
        }


        if (this.HeaderBorder.Visibility == Visibility.Collapsed)
        {
            this.HeaderBorder.Visibility = Visibility.Visible;
        }
    }

    public BreadCrumbBar() {
        InitializeComponent();
        BaseList = new ObservableCollection<TreeItemModel>();
        Binding binding = new Binding();
        binding.Source = this;
        binding.Path = new PropertyPath(nameof(BaseList));
        binding.Mode = BindingMode.TwoWay;
        navButton.SetBinding(ItemsControl.ItemsSourceProperty, binding);

        WeakReferenceMessenger.Default.Register<TreeItemModelMessenger>(this, PageAddItem);
       
    }

    private async void PageAddItem(object recipient, TreeItemModelMessenger message) {
        if (message.Item.Page is null) return;
        var windowOpen =
            Items.FirstOrDefault(e => e.Key == message.Item);
        if (windowOpen.Value is not null)
        {
            windowOpen.Value.Activate();
            windowOpen.Value.Focusable = true;
            return;
        }


        if (NaviControl.olditemModel is not null
            && message.Item == NaviControl.olditemModel && message.Item.PageStatus == PageStatus.Page)
        {
            Growl.Warning($"页面正在显示！");
            return;
        }


        if (this.DataContext is MainViewModel vm)
        {
            var url = $"{RegionName.HomeRegion}/{message.Item.Page}";
            await vm.NavigationService.NavigateAsync($"{RegionName.HomeRegion}/BasePage");
            var nav = await vm.NavigationService.NavigateAsync(url);
            if (!nav)
            {
                MessageBox.Show($"没有找到页面{url}");
                return;
            }
            else
            {
                if (NaviControl.olditemModel is not null && !NaviControl.olditemModel.IsPersistence)
                {
                    vm.NavigationService.ResetVm($"{RegionName.HomeRegion}/{NaviControl.olditemModel.Page}");
                }
            }
        }

        BaselistAdd(message.Item);
        NaviControl.olditemModel = message.Item;


        if (Application.Current.MainWindow != null &&
            Application.Current.MainWindow.WindowState == WindowState.Minimized)
        {
            Application.Current.MainWindow.WindowState = WindowState.Normal;
        }

        Application.Current.MainWindow?.Activate();

        WeakReferenceMessenger.Default.Send<NaviSendMessenger<TreeItemModel>>(
            new NaviSendMessenger<TreeItemModel>(message.Item)
        );
        NaviControl.olditemModel = message.Item;
    }

    private async void GotoView_Click(object sender, RoutedEventArgs e) {
        if ((sender as RadioButton)?.Tag is not TreeItemModel value) return;
        if (value.Page is null) return;
        if (this.DataContext is MainViewModel vm)
        {
            await vm.NavigationService.NavigateAsync($"{RegionName.HomeRegion}/{value.Page}");
        }

        WeakReferenceMessenger.Default.Send<NaviSendMessenger<TreeItemModel>>(
            new NaviSendMessenger<TreeItemModel>(value)
        );
    }

    private async void Eject_Click(object sender, RoutedEventArgs e) {
        if ((sender as Button)?.Tag is not TreeItemModel value) return;
        var nav = value.IsChecked;
        

        var page = await BaselistRemove(value);
        if (!nav) return;

        if (this.DataContext is MainViewModel vm && page is not null)
        {
            await vm.NavigationService.NavigateAsync($"{RegionName.HomeRegion}/{page.Page}");
        }
        WindowBase window = new WindowBase(value) { DataContext = this.DataContext };
        Items.Add(value, window);
        window.ShowAndActivated();
        WeakReferenceMessenger.Default.Send<NaviSendMessenger<TreeItemModel>>(
            new NaviSendMessenger<TreeItemModel>(page)
        );
        e.Handled = true;
    }

    private async void Close_Click(object sender, RoutedEventArgs e) {
        if ((sender as Button)?.Tag is not TreeItemModel value) return;
        var nav = value.IsChecked;
        var page = await BaselistRemove(value);
        if (!nav) return;
        if (this.DataContext is MainViewModel vm && page is not null)
        {
            await vm.NavigationService.NavigateAsync($"{RegionName.HomeRegion}/{page.Page}");
        }

        
        if (page is not null)
            WeakReferenceMessenger.Default.Send<NaviSendMessenger<TreeItemModel>>(
                new NaviSendMessenger<TreeItemModel>(page)
            );
    }
}