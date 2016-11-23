namespace AnyStatus
{
    public class AddFolderCommand : ItemCommand
    {
        public AddFolderCommand(Item item) : base(item) { }
    }

    public class AddFolderCommandHandler : IHandler<AddFolderCommand>
    {
        private readonly ISettingsStore _settingsStore;

        public AddFolderCommandHandler(ISettingsStore settingsStore)
        {
            _settingsStore = Preconditions.CheckNotNull(settingsStore, nameof(settingsStore));
        }

        public void Handle(AddFolderCommand command)
        {
            var item = command?.Item ?? _settingsStore.Settings.RootItem;

            if (item == null)
                return;

            var folder = new Folder
            {
                Name = "New Folder",
                IsEditing = true
            };

            item.Add(folder);

            item.IsExpanded = true;

            _settingsStore.TrySave();
        }
    }
}