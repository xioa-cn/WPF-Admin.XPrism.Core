using System.Windows;
using System.Windows.Controls;

namespace WPF.Admin.Themes.Controls;

public class Snackbar : Control
{
    static Snackbar()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(Snackbar), 
            new FrameworkPropertyMetadata(typeof(Snackbar)));
    }

    public static readonly DependencyProperty MessageProperty =
        DependencyProperty.Register("Message", typeof(string), typeof(Snackbar), 
            new PropertyMetadata(string.Empty));

    public string Message
    {
        get { return (string)GetValue(MessageProperty); }
        set { SetValue(MessageProperty, value); }
    }

    public static readonly DependencyProperty IsActiveProperty =
        DependencyProperty.Register("IsActive", typeof(bool), typeof(Snackbar), 
            new PropertyMetadata(false, OnIsActivePropertyChanged));

    public bool IsActive
    {
        get { return (bool)GetValue(IsActiveProperty); }
        set { SetValue(IsActiveProperty, value); }
    }

    public static readonly DependencyProperty DurationProperty =
        DependencyProperty.Register("Duration", typeof(TimeSpan), typeof(Snackbar), 
            new PropertyMetadata(TimeSpan.FromSeconds(3)));

    public TimeSpan Duration
    {
        get { return (TimeSpan)GetValue(DurationProperty); }
        set { SetValue(DurationProperty, value); }
    }

    private static void OnIsActivePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var snackbar = (Snackbar)d;
        if ((bool)e.NewValue)
        {
            snackbar.Show();
        }
        else
        {
            snackbar.Hide();
        }
    }

    private async void Show()
    {
        this.Visibility = Visibility.Visible;
        await Task.Delay(Duration);
        IsActive = false;
    }

    private void Hide()
    {
        this.Visibility = Visibility.Collapsed;
    }
}