using System.Windows.Controls;
using TopicModules.ViewModels;

namespace TopicModules.Views;

public partial class TopicView : Page {
    public TopicView() {
        InitializeComponent();
    }
    private void Themes_Click(object sender, System.Windows.RoutedEventArgs e) {
        if (sender is not Button button) return;
        var content = button.Content as string;
        (this.DataContext as TopicViewModel).Use(content);
    }
}