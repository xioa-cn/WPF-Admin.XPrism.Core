using System.Windows.Controls;
using InfiniteScrollingModules.ViewModel;

namespace InfiniteScrollingModules.Views;

public partial class InfiniteScrollingView : Page {
    public InfiniteScrollingView() {
        InitializeComponent();
    }
    private async void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e) {
        var scrollViewer = (ScrollViewer)sender;
        
        if (!(scrollViewer.VerticalOffset >= scrollViewer.ScrollableHeight)) return;
        if (this.DataContext == null || this.DataContext is not InfiniteScrollingViewModel viewModel) return;
      
        if (viewModel.CanMove)
        {
            await viewModel.MoveData();
        }
    }
}