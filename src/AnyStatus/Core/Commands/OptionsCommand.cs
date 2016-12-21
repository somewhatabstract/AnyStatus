using System;

namespace AnyStatus
{
    public class OptionsCommand : MenuCommandBase
    {
        private IPackage _package;
        private readonly ILogger _logger;

        public OptionsCommand(IPackage package, ILogger logger) : base(PackageIds.optionsToolbarCommandId)
        {
            _logger = logger;
            _package = package;
        }

        protected override void Handle(object sender, EventArgs e)
        {
            try
            {
                _package.ShowOptions();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to show options dialog.");
            }
        }
    }
}
