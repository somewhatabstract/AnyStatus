using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;

namespace AnyStatus.VSPackage
{
    class ToolWindowCmd : BaseCommand
    {
        private readonly Package _package;
        public ToolWindowCmd(Package package) : base(PackageIds.ToolWindowCommandId)
        {
            _package = package;
        }

        protected override void Handle(object sender, EventArgs e)
        {
            ToolWindowPane window = _package.FindToolWindow(typeof(ToolWindowHost), 0, true);

            if ((null == window) || (null == window.Frame))
            {
                throw new NotSupportedException("Cannot create tool window");
            }

            IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;

            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }
    }
}
