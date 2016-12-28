using System;

namespace AnyStatus
{
    public class ShowToolWindowCommand : MenuCommandBase
    {
        private readonly IPackage _package;

        public ShowToolWindowCommand(IPackage package) : base(PackageIds.ToolWindowCommandId)
        {
            _package = Preconditions.CheckNotNull(package, nameof(package));
        }

        protected override void Handle(object sender, EventArgs e)
        {
            _package.ShowToolWindow();
        }
    }
}
