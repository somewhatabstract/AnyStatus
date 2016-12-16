using System;

namespace AnyStatus.Features.ToolWindow.ToolbarCommands
{
    public class NewFolderCommand : ToolbarCommand
    {
        private readonly IMediator _mediator;
        private readonly ISettingsStore _settingsStore;

        public NewFolderCommand(IMediator mediator, ISettingsStore settingsStore) :base(PackageIds.addFolderToolbarCommandId)
        {
            _mediator = Preconditions.CheckNotNull(mediator, nameof(mediator));
            _settingsStore = Preconditions.CheckNotNull(settingsStore, nameof(settingsStore));
        }

        protected override void Handle(object sender, EventArgs e)
        {
            if (_settingsStore.Settings?.RootItem == null)
                return;

            var command = new AddFolderCommand(_settingsStore.Settings.RootItem);

            _mediator.Send(command);
        }
    }
}
