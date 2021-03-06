﻿using System;

namespace AnyStatus
{
    public class MoveDownCommand : ItemCommand
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
            if (command == null)
                throw new InvalidOperationException();

            command.Item.MoveDown();

            _settingsStore.TrySave();
        }
    }
}