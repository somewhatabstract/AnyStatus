using System;

namespace AnyStatus.VSPackage
{
    /// <summary>
    /// This command is called when AnyStatus menu command is called
    /// </summary>
    public class ShowToolWindow : ToolbarCommand
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
