using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace WPF.Admin.Themes.Converter;

public class IsFirstLevelConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
        {
            // 如果是null，说明是一级菜单
            return Visibility.Visible;
        }

        if (value is TreeViewItem treeViewItem)
        {
            var parent = GetParentTreeViewItem(treeViewItem);
            return parent == null ? Visibility.Visible : Visibility.Collapsed;
        }

        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return DependencyProperty.UnsetValue;
    }

    private TreeViewItem GetParentTreeViewItem(DependencyObject item)
    {
        var parent = VisualTreeHelper.GetParent(item);
        while (parent != null && !(parent is TreeViewItem))
        {
            parent = VisualTreeHelper.GetParent(parent);
        }
        return parent as TreeViewItem;
    }
}