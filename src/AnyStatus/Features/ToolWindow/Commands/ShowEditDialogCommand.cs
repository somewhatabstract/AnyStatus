using System;

namespace AnyStatus
{
    public class ShowEditDialogCommand : ItemCommand
    {
        public ShowEditDialogCommand(Item item) : base(item) { }
    }

    public class ShowEditDialogCommandHandler : IHandler<ShowEditDialogCommand>
    {
        private readonly EditWindow _view;
        private readonly EditViewModel _viewModel;

        public ShowEditDialogCommandHandler(EditWindow view, EditViewModel viewModel)
        {
            _view = Preconditions.CheckNotNull(view, nameof(view));

            _viewModel = Preconditions.CheckNotNull(viewModel, nameof(viewModel));

            _view.DataContext = _viewModel;

            _viewModel.CloseRequested += (s, e) => { _view.Close(); };
        }

        public void Handle(ShowEditDialogCommand command)
        {
            if (command == null)
                throw new InvalidOperationException();

            _viewModel.Item = command.Item;

            _view.ShowModal();
        }
    }
}