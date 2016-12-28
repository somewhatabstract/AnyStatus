using System;

namespace AnyStatus.Core.Commands
{
    public class ShowToolWindow : MenuCommandBase
    {
        private readonly IPackage _package;

        public ShowToolWindow(IPackage package) : base(PackageIds.ToolWindowCommandId)
        {
            _package = package;
        }

        protected override void Handle(object sender, EventArgs e)
        {
            _package.ShowToolWindow();
        }
    }
}
