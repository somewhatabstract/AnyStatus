using System;
using System.Diagnostics;
using System.Windows.Input;

namespace AnyStatus
{
    public class OpenInBrowserCommand : ICommand
    {
        private IMediator _mediator;

        public event EventHandler CanExecuteChanged;

        public OpenInBrowserCommand() : this(new Mediator())
        {
        }

        public OpenInBrowserCommand(IMediator mediator)
        {
            _mediator = Preconditions.CheckNotNull(mediator, nameof(mediator));
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var item = parameter as ICanOpenInBrowser;

            if (item == null)
                return;

            try
            {
                _mediator.Send(item, typeof(IOpenInBrowser<>));
            }
            catch
            {
            }
        }
    }
}