using System;

namespace AnyStatus
{
    public class ExpandAllCommand : MenuCommandBase
    {
        private readonly ISettingsStore _settingsStore;

        public ExpandAllCommand(ISettingsStore settingsStore) : base(PackageIds.expandToolbarCommandId)
        {
            _settingsStore = Preconditions.CheckNotNull(settingsStore, nameof(settingsStore));
        }

        protected override void Handle(object sender, EventArgs e)
        {
            _settingsStore.Settings?.RootItem?.ExpandAll();
        }
    }
}
