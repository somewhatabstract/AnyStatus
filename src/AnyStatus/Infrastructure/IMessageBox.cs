namespace AnyStatus
{
    public interface IMessageBox
    {
        System.Windows.MessageBoxResult Show(string text, string title, System.Windows.MessageBoxButton button, System.Windows.MessageBoxImage image);
    }
}