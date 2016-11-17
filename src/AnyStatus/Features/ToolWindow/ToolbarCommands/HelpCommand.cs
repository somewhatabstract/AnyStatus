using System;

namespace AnyStatus
{
    public class HelpCommand : BaseCommand
    {
        public HelpCommand() : base(PackageIds.helpToolbarCommandId)
        {
        }

        protected override void Handle(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("https://github.com/AlonAm/AnyStatus/wiki");
            }
            catch
            {
                // Ignore
            }
        }
    }
}
