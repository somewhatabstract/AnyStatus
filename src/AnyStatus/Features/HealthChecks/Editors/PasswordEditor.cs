using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

namespace AnyStatus
{
    public class PasswordEditor : ITypeEditor
    {
        PropertyItem _propertyItem;
        PasswordBox _passwordBox;

        public FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {
            _propertyItem = propertyItem;

            _passwordBox = new PasswordBox();
            _passwordBox.Password = (string)propertyItem.Value;
            _passwordBox.LostFocus += OnLostFocus;

            return _passwordBox;
        }

        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            if (!_passwordBox.Password.Equals((string)_propertyItem.Value))
            {
                _propertyItem.Value = _passwordBox.Password;
            }
        }
    }
}
