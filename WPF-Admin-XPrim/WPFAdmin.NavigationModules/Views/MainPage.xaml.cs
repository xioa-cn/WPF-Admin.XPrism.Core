using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using CommunityToolkit.Mvvm.Messaging;
using WPFAdmin.NavigationModules.Messengers;
using XPrism.Core.Navigations;

namespace WPFAdmin.NavigationModules.Views;

public partial class MainPage : Page,INavigationAware {
    public MainPage() {
        InitializeComponent();
        WeakReferenceMessenger.Default.Register<ChangeMainBorderSizeMessanger>(this, ChangeValue);
        this.Unloaded += (sender, args) =>
        {
            WeakReferenceMessenger.Default.Unregister<ChangeMainBorderSizeMessanger>(this);
        };
        PreviewKeyDown += (s, e) =>
        {
            if (e.Key == Key.Back)
            {
                // 尝试多种方式获取焦点元素
                IInputElement? focusedElement = null;

                // 1. 首先尝试 Keyboard.FocusedElement
                focusedElement = Keyboard.FocusedElement;

                // 2. 如果上面失败，尝试从事件源获取
                if (focusedElement == null && s is Window window)
                {
                    focusedElement = FocusManager.GetFocusedElement(window);
                }

                // 3. 如果还是失败，尝试从事件原始源获取
                if (focusedElement == null)
                {
                    focusedElement = e.OriginalSource as IInputElement;
                }

                // 4. 如果获取到了焦点元素，检查类型
                if (focusedElement != null)
                {
                    bool isEditableControl = focusedElement is TextBox || 
                                             focusedElement is PasswordBox || 
                                             focusedElement is RichTextBox ||
                                             (focusedElement is ComboBox combo && combo.IsEditable);

                    if (!isEditableControl)
                    {
                        e.Handled = true;
                    }
                }
            }
        };
    }

    private void ChangeValue(object recipient, ChangeMainBorderSizeMessanger message) {
        ChangeNavBorder();
    }

    public async Task OnNavigatingToAsync(INavigationParameters parameters) {
        
    }

    public async Task OnNavigatingFromAsync(INavigationParameters parameters) {
       
    }

    public async Task<bool> CanNavigateToAsync(INavigationParameters parameters) {
        return true;
    }

    public async Task<bool> CanNavigateFromAsync(INavigationParameters parameters) {
        return true;
    }

    private void OpenOrCloseNaviBar(object sender, RoutedEventArgs e) {
        ChangeNavBorder();
    }
    private void ChangeNavBorder()
    {
        var width = NaviGrid.Width;
        const double minWidth = 55.0;
        const double maxWidth = 200.0;

        DoubleAnimation doubleAnimation = new DoubleAnimation
        {
            Duration = new Duration(TimeSpan.FromSeconds(0.1)),
            From = width,
            To = Math.Abs(width - minWidth) < 0.01 ? maxWidth : minWidth
        };

        IconGrid.Visibility = Math.Abs(width - minWidth) < 0.01 ? Visibility.Collapsed : Visibility.Visible;
        NaviGrid.BeginAnimation(Border.WidthProperty, doubleAnimation);
    }
    private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
    {
        // 使用默认浏览器打开链接
        Process.Start(new ProcessStartInfo
        {
            FileName = e.Uri.AbsoluteUri,
            UseShellExecute = true
        });

        // 标记事件已处理
        e.Handled = true;
    }
}