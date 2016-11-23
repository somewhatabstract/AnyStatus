namespace AnyStatus
{
    public class AddCommand : ItemCommand
    {
        public AddCommand(Item item) : base(item) { }
    }

    public class AddCommandHandler : IHandler<AddCommand>
    {
        private readonly IViewLocator _viewLocator;

        public AddCommandHandler(IViewLocator viewLocator)
        {
            _viewLocator = Preconditions.CheckNotNull(viewLocator, nameof(viewLocator));
        }

        public void Handle(AddCommand command)
        {
            var selectedItem = command.Item;

            var dlg = _viewLocator.NewWindow(selectedItem);

            dlg.ShowModal();
        }
    }
}