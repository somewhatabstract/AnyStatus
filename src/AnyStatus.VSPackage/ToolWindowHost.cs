using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AnyStatus.VSPackage
{
    [Guid("f2878c76-3add-44b4-bf1e-84ec76e93873")]
    public class ToolWindowHost : ToolWindowPane
    {
        public ToolWindowHost() : base(null)
        {
            Caption = "AnyStatus";
            ToolBar = new CommandID(PackageGuids.guidToolWindowPackageCmdSet, PackageIds.ToolbarId);
            ToolBarLocation = (int)VSTWT_LOCATION.VSTWT_TOP;

            try
            {
                Content = TinyIoCContainer.Current.Resolve<ToolWindowControl>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}
