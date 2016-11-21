using System;
using System.Diagnostics;
using System.Windows.Input;

namespace AnyStatus
{
    public class OpenInBrowserCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var item = parameter as ICanOpenInBrowser;

            if (item == null)
                return;

            var handlerType = typeof(IOpenInBrowser<>);
            var genericHandlerType = handlerType.MakeGenericType(item.GetType());
            var handler = TinyIoCContainer.Current.Resolve(genericHandlerType);

            genericHandlerType.GetMethod("Handle").Invoke(handler, new[] { item });
        }
    }
}