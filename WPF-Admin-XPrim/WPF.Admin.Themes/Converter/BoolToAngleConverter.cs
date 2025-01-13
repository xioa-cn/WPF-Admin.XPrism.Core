using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WPF.Admin.Themes.Converter;

public class BoolToAngleConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if (value is bool isChecked)
        {
            return isChecked ? 180 : 0;
        }

        return 0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        return DependencyProperty.UnsetValue;
    }
}