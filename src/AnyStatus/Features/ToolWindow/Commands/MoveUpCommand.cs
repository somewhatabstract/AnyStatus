namespace AnyStatus
{
    public class MoveUpCommand : ItemCommand
    {
        public MoveUpCommand(Item item) : base(item) { }
    }

    public class MoveUpCommandHandler : IHandler<MoveUpCommand>
    {
        private readonly ISettingsStore _settingsStore;

        public MoveUpCommandHandler(ISettingsStore settingsStore)
        {
            _settingsStore = Preconditions.CheckNotNull(settingsStore, nameof(settingsStore)); ;
        }

        public void Handle(MoveUpCommand command)
        {
            if (command.Item == null)
                return;

            command.Item.MoveUp();

            _settingsStore.TrySave();
        }
    }
}