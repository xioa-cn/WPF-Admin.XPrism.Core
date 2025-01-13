using System.Globalization;
using System.Windows;
using System.Windows.Data;
using WPF.Admin.Themes.Themes;

namespace WPF.Admin.Themes.Converter;

public class IconConvert : IValueConverter {
    private static IconPaths _iconPaths = new IconPaths();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
        return _iconPaths[(string)value];
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
        return DependencyProperty.UnsetValue;
    }
}