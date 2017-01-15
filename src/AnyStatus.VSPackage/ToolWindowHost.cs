using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AnyStatus.VSPackage
{
    [Guid("f2878c76-3add-44b4-bf1e-84ec76e93873")]
    public class ToolWindowHost : ToolWindowPane, IToolWindow
    {
        private bool _infoBarIsActive;

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

        public void AddInfoBar()
        {
            if (_infoBarIsActive)
                return;

            var infoBar = new InfoBarModel(
                textSpans: new[]
                {
                    new InfoBarTextSpan("The configuration file has been changed.")
                },
                actionItems: new[]
                {
                    new InfoBarButton("Reload"),
                    new InfoBarButton("Dismiss")
                },
                image: KnownMonikers.StatusInformation,
                isCloseButtonVisible: true);

            ThreadHelper.JoinableTaskFactory.Run(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                AddInfoBar(infoBar);

                InfoBarActionItemClicked += OnInfoBarActionItemClicked;
                InfoBarClosed += OnInfoBarClosed;
                _infoBarIsActive = true;
            });
        }

        private void OnInfoBarClosed(object sender, InfoBarEventArgs e)
        {
            _infoBarIsActive = false;
            InfoBarClosed -= OnInfoBarClosed;
        }

        private void OnInfoBarActionItemClicked(object sender, InfoBarActionItemEventArgs e)
        {
            if (e.ActionItem.Text == "Reload")
            {
                //TinyIoCContainer.Current.Resolve<ILogger>().Info("Reload");
            }

            RemoveInfoBar(e.InfoBarModel);

            InfoBarActionItemClicked -= OnInfoBarActionItemClicked;
        }


    }
}
