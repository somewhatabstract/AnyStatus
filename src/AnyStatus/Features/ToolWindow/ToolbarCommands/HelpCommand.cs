using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace AnyStatus
{
    [ExcludeFromCodeCoverage]
    public class HelpCommand : BaseCommand
    {
        public HelpCommand() : base(PackageIds.helpToolbarCommandId)
        {
        }

        protected override void Handle(object sender, EventArgs e)
        {
            try
            {
                Process.Start("https://github.com/AlonAm/AnyStatus/wiki");
            }
            catch
            {
            }
        }
    }
}
