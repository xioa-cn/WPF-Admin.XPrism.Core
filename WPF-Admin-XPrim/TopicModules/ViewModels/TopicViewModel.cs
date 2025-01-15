using CommunityToolkit.Mvvm.Input;
using WPF.Admin.Models;
using WPF.Admin.Themes;

namespace TopicModules.ViewModels;

public partial class TopicViewModel : BindableBase {
    public string[] Themes { get; set; } = {
        "Green", "Purple", "Red", "Yellow", "Pink", "Orange", "Cyan", "Blue",
        "DarkGreen", "DarkPurple", "DarkRed", "DarkYellow", "DarkPink", "DarkOrange", "DarkCyan", "DarkBlue",
    };


    [RelayCommand]
    public void Use(string? content) {
        if (string.IsNullOrEmpty(content)) return;
        ThemeManager.UseTheme(content);
    }
}