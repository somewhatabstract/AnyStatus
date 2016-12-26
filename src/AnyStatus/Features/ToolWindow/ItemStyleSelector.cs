using System.Windows;
using System.Windows.Controls;

namespace AnyStatus
{
    public class ItemStyleSelector : StyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {
            var element = container as FrameworkElement;

            if (item == null || element == null)
                return null;

            if (item is Folder)
                return element.FindResource("treeViewFolderStyle") as Style;

            return element.FindResource("treeViewItemStyle") as Style;
        }
    }
}
