namespace AnyStatus
{
    public class MoveDownCommand : BaseItemCommand
    {
        public MoveDownCommand(Item item) : base(item) { }
    }

    public class MoveDownCommandHandler : IHandler<MoveDownCommand>
    {
        private readonly ISettingsStore _settingsStore;

        public MoveDownCommandHandler(ISettingsStore settingsStore)
        {
            _settingsStore = Preconditions.CheckNotNull(settingsStore, nameof(settingsStore)); ;
        }

        public void Handle(MoveDownCommand command)
        {
            if (command.Item == null)
                return;

            command.Item.MoveDown();

            _settingsStore.TrySave();
        }
    }
}