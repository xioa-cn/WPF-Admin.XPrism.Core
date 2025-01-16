
using TopicModules.ViewModels;

namespace TopicModules.Views;

public partial class TopicView :System.Windows.Controls. UserControl {
    public TopicView() {
        InitializeComponent();
    }
    private void Themes_Click(object sender, System.Windows.RoutedEventArgs e) {
        if (sender is not System.Windows.Controls.Button button) return;
        var content = button.Content as string;
        (this.DataContext as TopicViewModel).Use(content);
    }
}