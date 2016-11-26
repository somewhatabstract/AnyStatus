using System.Windows;

namespace AnyStatus
{
    public interface IDialogService
    {
        MessageBoxResult Show(string text, string title, MessageBoxButton button, MessageBoxImage image);
    }
}