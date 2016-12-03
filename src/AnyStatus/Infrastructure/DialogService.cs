using System.Windows;

namespace AnyStatus
{
    public class DialogService : IDialogService
    {
        public MessageBoxResult Show(string text, string title, MessageBoxButton button, MessageBoxImage image)
        {
            return MessageBox.Show(text, title, button, image);
        }

        public void ShowWarning(string message, string caption)
        {
            MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        public void ShowInfo(string message, string caption)
        {
            MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
