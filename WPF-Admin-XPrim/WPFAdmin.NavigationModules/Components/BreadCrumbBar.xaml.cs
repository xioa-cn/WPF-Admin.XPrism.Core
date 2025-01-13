using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using CommunityToolkit.Mvvm.Messaging;
using HandyControl.Controls;
using WPF.Admin.Models.Models;
using WPFAdmin.NavigationModules.Messengers;
using WPFAdmin.NavigationModules.ViewModel;
using XPrism.Core.DI;
using MessageBox = HandyControl.Controls.MessageBox;

namespace WPFAdmin.NavigationModules.Components;

public partial class BreadCrumbBar : UserControl
{
    public static Dictionary<TreeItemModel, System.Windows.Window> items { get; set; } =
        new Dictionary<TreeItemModel, System.Windows.Window>();


    private ObservableCollection<TreeItemModel>? _baseItem;

    public ObservableCollection<TreeItemModel> BaseList
    {
        get => _baseItem;
        set { _baseItem = value; }
    }

    private TreeItemModel BaselistRemove(TreeItemModel value)
    {
        var index = Array.IndexOf(BaseList.ToArray(), value);
        TreeItemModel? page = new TreeItemModel();

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

        value.IsChecked = false;
        this.BaseList.Remove(value);
        //items.Remove(value);
        if (this.BaseList.Count < 1)
        {
            this.HeaderBorder.Visibility = Visibility.Collapsed;
            if (this.DataContext is MainViewModel vm)
            {
                vm.NavigationService.NavigateAsync("Home/BasePage");
            }
        }

        return page;
    }

    private void BaselistAdd(TreeItemModel value)
    {
        var r = this.BaseList.FirstOrDefault(x => x == value);
        if (r is not null)
        {
            r.IsChecked = true;
        }
        else
        {
            this.BaseList.Add(value);
            //items.Add(value);
        }


        if (this.HeaderBorder.Visibility == Visibility.Collapsed)
        {
            this.HeaderBorder.Visibility = Visibility.Visible;
        }
    }

    public BreadCrumbBar()
    {
        InitializeComponent();
        BaseList = new ObservableCollection<TreeItemModel>();
        Binding binding = new Binding();
        binding.Source = this;
        binding.Path = new PropertyPath(nameof(BaseList));
        binding.Mode = BindingMode.TwoWay;
        navButton.SetBinding(ItemsControl.ItemsSourceProperty, binding);

        WeakReferenceMessenger.Default.Register<TreeItemModelMessenger>(this, PageAddItem);
    }

    private async void PageAddItem(object recipient, TreeItemModelMessenger message)
    {
        await this.Dispatcher.InvokeAsync(async () =>
         {
             if (message.Item.Page is null) return;
             var windowOpen =
                 items.FirstOrDefault(e => e.Key == message.Item);
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
                 string url = $"{RegionName.HomeRegion}/{message.Item.Page}";
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
             else
             {

             }

             // var temp = XPrismIoc.Fetch(message.Item.Page);
             //
             //
             // message.Item.Page = temp as Page;


             BaselistAdd(message.Item);
             NaviControl.olditemModel = message.Item;



             if (Application.Current.MainWindow.WindowState == WindowState.Minimized)
             {
                 Application.Current.MainWindow.WindowState = WindowState.Normal;
             }

             Application.Current.MainWindow.Activate();
         });
        WeakReferenceMessenger.Default.Send<NaviSendMessenger<TreeItemModel>>(
            new NaviSendMessenger<TreeItemModel>(message.Item)
        );
        NaviControl.olditemModel = message.Item;
    }

    private void GotoView_Click(object sender, RoutedEventArgs e)
    {
    }

    private void Eject_Click(object sender, RoutedEventArgs e)
    {
    }

    private void Close_Click(object sender, RoutedEventArgs e)
    {
    }
}