using System.Globalization;
using System.Windows;
using System.Windows.Data;
using WPF.Admin.Models.Models;

namespace WPF.Admin.Themes.Converter;

public class LoginAuthToEnabledConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (LoginAuthHelper.ViewAuthSwitch == ViewAuthSwitch.Visibility)
            return true;
        
        if (value is LoginAuth requiredAuth && LoginAuthHelper.LoginUser != null)
        {
            return (int)LoginAuthHelper.LoginUser.LoginAuth >= (int)requiredAuth;
        }
        return true;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return DependencyProperty.UnsetValue;
    }
}