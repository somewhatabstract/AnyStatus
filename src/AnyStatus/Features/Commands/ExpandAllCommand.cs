using AnyStatus.Interfaces;
using System;

namespace AnyStatus
{
    public class ExpandAllCommand : BaseCommand
    {
        private readonly IUserSettings _userSettings;

        public ExpandAllCommand(IUserSettings userSettings) : base(PackageIds.expandToolbarCommandId)
        {
            _userSettings = userSettings;
        }

        protected override void Handle(object sender, EventArgs e)
        {
            _userSettings.RootItem?.ExpandAll();
        }
    }
}
