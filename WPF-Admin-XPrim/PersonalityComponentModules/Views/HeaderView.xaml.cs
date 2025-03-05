using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace PersonalityComponentModules.Views;

public partial class HeaderView : Page {
    public HeaderView() {
        InitializeComponent();
    }

    private void AvatarContainer_MouseEnter(object sender, MouseEventArgs e)
    {
        Storyboard storyboard = FindResource("MouseEnterAnimation") as Storyboard;
        storyboard?.Begin();
    }

    private void AvatarContainer_MouseLeave(object sender, MouseEventArgs e)
    {
        Storyboard storyboard = FindResource("MouseLeaveAnimation") as Storyboard;
        storyboard?.Begin();
    }
}