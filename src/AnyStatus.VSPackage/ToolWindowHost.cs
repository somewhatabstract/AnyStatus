using AnyStatus.Infrastructure;
using AnyStatus.Views;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;

namespace AnyStatus.VSPackage
{
    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    /// </summary>
    /// <remarks>
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane,
    /// usually implemented by the package implementer.
    /// <para>
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its
    /// implementation of the IVsUIElementPane interface.
    /// </para>
    /// </remarks>
    [Guid("f2878c76-3add-44b4-bf1e-84ec76e93873")]
    public class ToolWindowHost : ToolWindowPane
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ToolWindowHost"/> class.
        /// </summary>
        public ToolWindowHost() : base(null)
        {
            Caption = "AnyStatus";

            ToolBar = new CommandID(new Guid("{c420854f-cac2-4492-8067-ecf632228390}"), VSPackage.ToolbarId);
            ToolBarLocation = (int)VSTWT_LOCATION.VSTWT_TOP;

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.
            Content = TinyIoCContainer.Current.Resolve<ToolWindowControl>();
        }
    }
}
