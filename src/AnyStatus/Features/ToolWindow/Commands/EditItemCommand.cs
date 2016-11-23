namespace AnyStatus
{
    public class EditCommand : ItemCommand
    {
        public EditCommand(Item item) : base(item) { }
    }

    public class EditCommandHandler : IHandler<EditCommand>
    {
        private readonly IViewLocator _viewLocator;

        public EditCommandHandler(IViewLocator viewLocator)
        {
            _viewLocator = Preconditions.CheckNotNull(viewLocator, nameof(viewLocator));
        }

        public void Handle(EditCommand command)
        {
            var selectedItem = command.Item;

            var dlg = _viewLocator.EditWindow(selectedItem);

            dlg.ShowModal();
        }
    }
}