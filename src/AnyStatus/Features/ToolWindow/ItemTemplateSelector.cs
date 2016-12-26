using System.Windows;
using System.Windows.Controls;

namespace AnyStatus
{
    public class ItemTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return SelectTemplate(item as Item, container);
        }

        private DataTemplate SelectTemplate(Item item, DependencyObject container)
        {
            var element = container as FrameworkElement;

            if (item == null || element == null)
                return null;

            if (item is Folder)
                return element.FindResource("folderTemplate") as DataTemplate;

            if (item is Metric)
                return element.FindResource("metricTemplate") as DataTemplate;

            return element.FindResource("itemTemplate") as DataTemplate;
        }
    }
}
