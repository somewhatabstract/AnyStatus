using System.Windows;

namespace AnyStatus
{
    public class DialogService : IDialogService
    {
        public MessageBoxResult Show(string text, string title, MessageBoxButton button, MessageBoxImage image)
        {
            return MessageBox.Show(text, title, button, image);
        }
    }
}
