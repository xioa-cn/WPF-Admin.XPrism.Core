﻿using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;
using WPF.Admin.Models.Utils;

namespace WPF.Admin.Models.Models;

public partial class TreeItemModel : BindableBase {
    /// <summary>
    /// 持久化使用组件
    /// </summary>
    [JsonPropertyName("isPersistence")]
    public bool IsPersistence { get; set; } = true;

    [JsonPropertyName("content")] public string? Content { get; set; }
    [JsonPropertyName("loginAuth")] public LoginAuth LoginAuth { get; set; }
    [JsonPropertyName("icon")] public string? Icon { get; set; }
    [JsonPropertyName("page")] public string? Page { get; set; }

    [ObservableProperty] private bool _isChecked;

    [ObservableProperty] private bool _isExpanded;
    [JsonPropertyName("children")] public ObservableCollection<TreeItemModel> Children { get; set; } = new();

    private bool _isEnabled = true;


    public bool IsEnabled {
        get => _isEnabled;
        set
        {
            if(AuthHelper.ViewAuthSwitch != ViewAuthSwitch.IsEnabled) return;
            _isEnabled = value;
            OnPropertyChanged(nameof(IsEnabled));
        }
    }

    private bool _visibility = true;

    public bool Visibility {
        get => _visibility;
        set
        {
            if (AuthHelper.ViewAuthSwitch != ViewAuthSwitch.Visibility) return;
            _visibility = value;
            OnPropertyChanged(nameof(Visibility));
        }
    }

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