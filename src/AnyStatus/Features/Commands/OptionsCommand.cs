using Microsoft.VisualStudio.Shell;
using System;

namespace AnyStatus
{
    public class OptionsCommand : BaseCommand
    {
        private Package _package;
        private readonly ILogger _logger;

        public OptionsCommand(Package package, ILogger logger) :
            base(PackageIds.optionsToolbarCommandId)
        {
            _logger = logger;
            _package = package;
        }

        protected override void Handle(object sender, EventArgs e)
        {
            try
            {
                _package.ShowOptionPage(typeof(Options));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to open options dialog");
            }
        }
    }
}
