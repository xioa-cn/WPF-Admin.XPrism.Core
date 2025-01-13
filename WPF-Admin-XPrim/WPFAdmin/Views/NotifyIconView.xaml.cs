using System.Windows.Controls;
using WPFAdmin.ViewModels;

namespace WPFAdmin.Views;

public partial class NotifyIconView : UserControl {
    public NotifyIconView() {
        this.DataContext = new NotifyIconViewModel();
        InitializeComponent();
    }
}