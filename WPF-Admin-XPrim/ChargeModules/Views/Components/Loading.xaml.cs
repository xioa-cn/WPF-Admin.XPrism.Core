using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ChargeModules.Views.Components;

public partial class Loading : UserControl
{
    private Random random = new Random();
    private const int BubbleCount = 12;

    public Loading()
    {
        InitializeComponent();
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        // 使用脉动动画
        Storyboard pulseStoryboard = (Storyboard)FindResource("PulseAnimation");
        pulseStoryboard.Begin();

        // 启动旋转动画
        Storyboard rotateStoryboard = (Storyboard)FindResource("RotateAnimation");
        rotateStoryboard.Begin();
        
        // 启动内圈反向旋转动画
        Storyboard innerRotateStoryboard = (Storyboard)FindResource("InnerRotateAnimation");
        innerRotateStoryboard.Begin();

        //Storyboard glowStoryboard = (Storyboard)FindResource("GlowAnimation");
        //glowStoryboard.Begin();

        Storyboard bubblesStoryboard = (Storyboard)FindResource("BubblesAnimation");
        bubblesStoryboard.Begin();
    }
}