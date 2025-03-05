using System.Windows.Input;
using System.Windows;

namespace WPF.Admin.Themes.Attached
{
    public class MouseButtonBehavior
    {
        private static bool _isMouseDown = false;
        private static FrameworkElement _sourceElement = null;

        #region MouseLeftButtonDownBehavior
        public static readonly DependencyProperty MouseLeftButtonDownBehaviorProperty =
            DependencyProperty.RegisterAttached("MouseLeftButtonDownBehavior", typeof(ICommand), typeof(MouseButtonBehavior),
                new PropertyMetadata(null, OnMouseLeftButtonDownBehaviorChanged));

        public static readonly DependencyProperty MouseLeftButtonDownParameterProperty =
            DependencyProperty.RegisterAttached("MouseLeftButtonDownParameter", typeof(object), typeof(MouseButtonBehavior),
                new PropertyMetadata(null));

        public static void SetMouseLeftButtonDownBehavior(DependencyObject element, ICommand value)
        {
            element.SetValue(MouseLeftButtonDownBehaviorProperty, value);
        }

        public static ICommand GetMouseLeftButtonDownBehavior(DependencyObject element)
        {
            return (ICommand)element.GetValue(MouseLeftButtonDownBehaviorProperty);
        }

        public static void SetMouseLeftButtonDownParameter(DependencyObject element, object value)
        {
            element.SetValue(MouseLeftButtonDownParameterProperty, value);
        }

        public static object GetMouseLeftButtonDownParameter(DependencyObject element)
        {
            return element.GetValue(MouseLeftButtonDownParameterProperty);
        }
        #endregion

        #region MouseLeftButtonUpBehavior
        public static readonly DependencyProperty MouseLeftButtonUpBehaviorProperty =
            DependencyProperty.RegisterAttached("MouseLeftButtonUpBehavior", typeof(ICommand), typeof(MouseButtonBehavior),
                new PropertyMetadata(null, OnMouseLeftButtonUpBehaviorChanged));

        public static readonly DependencyProperty MouseLeftButtonUpParameterProperty =
            DependencyProperty.RegisterAttached("MouseLeftButtonUpParameter", typeof(object), typeof(MouseButtonBehavior),
                new PropertyMetadata(null));

        public static void SetMouseLeftButtonUpBehavior(DependencyObject element, ICommand value)
        {
            element.SetValue(MouseLeftButtonUpBehaviorProperty, value);
        }

        public static ICommand GetMouseLeftButtonUpBehavior(DependencyObject element)
        {
            return (ICommand)element.GetValue(MouseLeftButtonUpBehaviorProperty);
        }

        public static void SetMouseLeftButtonUpParameter(DependencyObject element, object value)
        {
            element.SetValue(MouseLeftButtonUpParameterProperty, value);
        }

        public static object GetMouseLeftButtonUpParameter(DependencyObject element)
        {
            return element.GetValue(MouseLeftButtonUpParameterProperty);
        }
        #endregion

        private static void OnMouseLeftButtonDownBehaviorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement element)
            {
                element.PreviewMouseLeftButtonDown += Element_PreviewMouseLeftButtonDown;
            }
        }

        private static void OnMouseLeftButtonUpBehaviorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement element)
            {
                element.PreviewMouseLeftButtonUp += Element_PreviewMouseLeftButtonUp;
            }
        }

        private static void Element_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element)
            {
                _isMouseDown = true;
                _sourceElement = element;
                var command = GetMouseLeftButtonDownBehavior(element);
                var parameter = GetMouseLeftButtonDownParameter(element);
                if (command != null && command.CanExecute(parameter))
                {
                    command.Execute(parameter);
                }
                e.Handled = true;
            }
        }

        private static void Element_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isMouseDown && sender is FrameworkElement element && element == _sourceElement)
            {
                var command = GetMouseLeftButtonUpBehavior(element);
                var parameter = GetMouseLeftButtonUpParameter(element);
                if (command != null && command.CanExecute(parameter))
                {
                    command.Execute(parameter);
                }
                _isMouseDown = false;
                _sourceElement = null;
                e.Handled = true;
            }
        }
    }
}
