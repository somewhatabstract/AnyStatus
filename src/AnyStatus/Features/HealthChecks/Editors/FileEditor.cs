using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

namespace AnyStatus
{
    public class FileEditor : ITypeEditor
    {
        public FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {
            Grid grid = new Grid();

            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

            TextBox textBox = new TextBox();
            textBox.HorizontalAlignment = HorizontalAlignment.Stretch;

            Binding binding = new Binding("Value"); //bind to the Value property of the PropertyItem
            binding.Source = propertyItem;
            binding.Mode = BindingMode.TwoWay;
            BindingOperations.SetBinding(textBox, TextBox.TextProperty, binding);

            Button button = new Button();
            button.Content = "...";
            button.Tag = propertyItem;
            button.Click += button_Click;
            Grid.SetColumn(button, 1);

            grid.Children.Add(textBox);
            grid.Children.Add(button);

            return grid;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            PropertyItem item = ((Button)sender).Tag as PropertyItem;

            if (null == item) return;

            var openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();
            item.Value = openFileDialog.FileName;
        }
    }

}
