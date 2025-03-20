using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace WPF.Admin.Themes.Controls;

public class SnackbarAdorner : Adorner
{
    private readonly Snackbar _snackbar;

    public SnackbarAdorner(UIElement adornedElement, Snackbar snackbar)
        : base(adornedElement)
    {
        _snackbar = snackbar;
        AddVisualChild(_snackbar);
    }

    protected override Visual GetVisualChild(int index)
    {
        return _snackbar;
    }

    protected override int VisualChildrenCount
    {
        get { return 1; }
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        _snackbar.Arrange(new Rect(new Point((finalSize.Width - _snackbar.DesiredSize.Width) / 2,
                finalSize.Height - _snackbar.DesiredSize.Height - 20),
            _snackbar.DesiredSize));
        return finalSize;
    }

    protected override Size MeasureOverride(Size constraint)
    {
        _snackbar.Measure(constraint);
        return constraint;
    }
}