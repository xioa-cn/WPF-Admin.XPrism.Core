using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WPF.Admin.Themes.Converter;

internal class ConvertToDoubleConverter : IValueConverter
{
    private double increment;

    public double Increment
    {
        get => increment;
        set => increment = value;
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return increment + (double)value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return DependencyProperty.UnsetValue;
    }
}