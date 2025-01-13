﻿using System.Windows;
using System.Windows.Media.Animation;

namespace WPFAdmin.Views;

public partial class SplashScreen : Window {
    public SplashScreen() {
        InitializeComponent();
        Loaded += SplashScreen_Loaded;
    }
    
    private void SplashScreen_Loaded(object sender, RoutedEventArgs e)
    {
        var storyboard = FindResource("LoadingAnimation") as Storyboard;
        storyboard?.Begin();
    }
}