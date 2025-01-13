using System.Globalization;
using System.Windows;
using System.Windows.Data;
using WPF.Admin.Models.Models;


namespace WPF.Admin.Themes.Converter;

public class LoginAuthToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (LoginAuthHelper.ViewAuthSwitch == ViewAuthSwitch.IsEnabled)
            return Visibility.Visible;

        if (value is LoginAuth requiredAuth && LoginAuthHelper.LoginUser != null)
        {
            return (int)LoginAuthHelper.LoginUser.LoginAuth >= (int)requiredAuth
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}