namespace AnyStatus
{
    public class MessageBox : IMessageBox
    {
        public virtual System.Windows.MessageBoxResult Show(string text, string title, System.Windows.MessageBoxButton button, System.Windows.MessageBoxImage image)
        {
            return System.Windows.MessageBox.Show(text, title, button, image);
        }
    }
}
