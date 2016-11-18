using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

namespace AnyStatus
{
    public class PasswordEditor : ITypeEditor
    {
        PasswordBox _passwordBox;
        PropertyItem _propertyItem;

        public FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {
            _propertyItem = propertyItem;

            _passwordBox = new PasswordBox
            {
                BorderThickness = new Thickness(0),
                Password = (string)_propertyItem.Value
            };

            _passwordBox.LostFocus += OnLostFocus;

            return _passwordBox;
        }

        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (!_passwordBox.Password.Equals((string)_propertyItem.Value))
            {
                _propertyItem.Value = _passwordBox.Password;
            }

            e.Handled = true;
        }
    }
}
