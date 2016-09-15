using AnyStatus.Interfaces;
using System;

namespace AnyStatus
{
    public class CollapseAllCommand : BaseCommand
    {
        private readonly IUserSettings _userSettings;

        public CollapseAllCommand(IUserSettings userSettings) : base(PackageIds.collapseToolbarCommandId)
        {
            _userSettings = userSettings;
        }

        protected override void Handle(object sender, EventArgs e)
        {
            _userSettings.RootItem?.CollapseAll();
        }
    }
}
