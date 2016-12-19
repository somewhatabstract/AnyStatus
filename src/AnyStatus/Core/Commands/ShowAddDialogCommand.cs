using System;

namespace AnyStatus
{
    public class ShowAddDialogCommand : ItemCommand
    {
        public ShowAddDialogCommand(Item item) : base(item) { }
    }

    public class ShowAddDialogCommandHandler : IHandler<ShowAddDialogCommand>
    {
        private readonly NewWindow _view;
        private readonly NewViewModel _viewModel;

        public ShowAddDialogCommandHandler(NewWindow view, NewViewModel viewModel)
        {
            _view = Preconditions.CheckNotNull(view, nameof(view));

            _viewModel = Preconditions.CheckNotNull(viewModel, nameof(viewModel));

            _view.DataContext = _viewModel;

            _viewModel.CloseRequested += (s, e) => { _view.Close(); };
        }

        public void Handle(ShowAddDialogCommand command)
        {
            if (command == null)
                throw new InvalidOperationException();

            _viewModel.Parent = command.Item;

            _view.ShowModal();
        }
    }
}