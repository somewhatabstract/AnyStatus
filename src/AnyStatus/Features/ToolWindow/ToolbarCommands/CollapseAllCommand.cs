using System;

namespace AnyStatus
{
    public class CollapseAllCommand : ToolbarCommand
    {
        private readonly ISettingsStore _settingsStore;

        public CollapseAllCommand(ISettingsStore settingsStore) : base(PackageIds.collapseToolbarCommandId)
        {
            _settingsStore = Preconditions.CheckNotNull(settingsStore, nameof(settingsStore));
        }

        protected override void Handle(object sender, EventArgs e)
        {
            _settingsStore.Settings?.RootItem?.CollapseAll();
        }
    }
}
