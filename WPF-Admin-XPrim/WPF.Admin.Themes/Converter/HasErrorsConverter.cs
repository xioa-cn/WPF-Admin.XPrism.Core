using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WPF.Admin.Themes.Converter;

public class HasErrorsConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if (value is bool hasErrors)
        {
            return !hasErrors;
        }

        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        return DependencyProperty.UnsetValue;
    }
}