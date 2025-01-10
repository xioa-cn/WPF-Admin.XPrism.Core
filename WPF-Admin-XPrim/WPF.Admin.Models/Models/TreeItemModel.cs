using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace WPF.Admin.Models.Models;

public partial class TreeItemModel : BindableBase {
    /// <summary>
    /// 持久化使用组件
    /// </summary>
    public bool IsPersistence { get; set; } = true;

    public string? Content { get; set; }

    public LoginAuth LoginAuth { get; set; }

    public object? Icon { get; set; }

    public Type? Page { get; set; }

    [ObservableProperty] private bool _isChecked;

    [ObservableProperty] private bool _isExpanded;

    public ObservableCollection<TreeItemModel> Children { get; set; } = new();

    public bool HasChildren => Children.Count > 0;

    public PageCanInterchange PageCanInterchange { get; set; } = PageCanInterchange.Can;

    public TreeItemModel() {
    }

    [ObservableProperty] private string? _message;

    public bool HasMessage => !string.IsNullOrEmpty(_message);

    public PageStatus PageStatus { get; set; } = PageStatus.Page;

    public TreeItemModel(string con) {
        this.Content = con;
    }
}