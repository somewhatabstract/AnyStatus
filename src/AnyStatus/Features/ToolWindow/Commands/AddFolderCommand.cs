using System;

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
            if (command == null)
                throw new InvalidOperationException();

            var folder = new Folder
            {
                IsEditing = true,
                Name = "New Folder"
            };

            command.Item.Add(folder);

            command.Item.IsExpanded = true;

            _settingsStore.TrySave();
        }
    }
}