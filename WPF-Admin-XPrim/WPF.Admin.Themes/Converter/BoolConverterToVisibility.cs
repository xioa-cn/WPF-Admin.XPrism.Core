using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WPF.Admin.Themes.Converter;

class BoolConverterToVisibility : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if (value is bool b)
        {
            return b ? Visibility.Visible : Visibility.Collapsed;
        }

        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        return DependencyProperty.UnsetValue;
    }
}