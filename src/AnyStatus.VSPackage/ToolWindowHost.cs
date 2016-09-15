using AnyStatus.Infrastructure;
using AnyStatus.Views;
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
                // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
                // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
                // the object returned by the Content property.
                Content = TinyIoCContainer.Current.Resolve<ToolWindowControl>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

        }
    }
}
