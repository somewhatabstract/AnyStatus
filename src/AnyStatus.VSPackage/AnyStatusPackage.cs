using Microsoft.VisualStudio.Shell;
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
    public sealed class AnyStatusPackage : Package
    {
        private AnyStatusApp _app;

        protected override async void Initialize()
        {
            base.Initialize();

            try
            {
                var container = ContainerBuilder.Build(this);

                _app = container.Resolve<AnyStatusApp>();

                await _app.InitializeAsync().ConfigureAwait(false);

                _app.Start();
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
    }
}
