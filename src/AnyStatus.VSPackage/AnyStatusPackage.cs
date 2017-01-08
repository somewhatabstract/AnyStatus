using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace AnyStatus.VSPackage
{
    [ProvideBindingPath]
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(ToolWindowHost))]
    [Guid(PackageGuids.guidToolWindowPackageString)]
    [ProvideOptionPage(typeof(GeneralOptions), "AnyStatus", "General", 0, 0, true)]
    [ProvideOptionPage(typeof(UserInterfaceOptions), "AnyStatus", "User Interface", 0, 1, true)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    public sealed class AnyStatusPackage : Package, IPackage
    {
        private AnyStatusApp _app;
        private EnvDTE.DTEEvents _events;

        protected override void Initialize()
        {
            base.Initialize();

            try
            {
                var container = ContainerBuilder.Build(this);

                container.Resolve<ICommandRegistry>().RegisterCommands();

                _app = container.Resolve<AnyStatusApp>();

                KnownUIContexts.ShellInitializedContext.WhenActivated(() =>
                {
                    System.Threading.Tasks.Task.Run(() =>
                    {
                        _app.Start();
                    });
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        protected override int QueryClose(out bool canClose)
        {
            _app?.Stop();

            return base.QueryClose(out canClose);
        }

        public void ShowOptions()
        {
            ShowOptionPage(typeof(GeneralOptions));
        }

        public void ShowToolWindow()
        {
            ToolWindowPane window = FindToolWindow(typeof(ToolWindowHost), 0, true);

            if ((null == window) || (null == window.Frame))
            {
                throw new NotSupportedException("Cannot create tool window");
            }

            IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;

            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }
    }
}
