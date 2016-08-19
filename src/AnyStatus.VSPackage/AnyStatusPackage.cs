using AnyStatus.Infrastructure;
using AnyStatus.Views;
using FluentScheduler;
using Microsoft.VisualStudio.Shell;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace AnyStatus.VSPackage
{
    [ProvideBindingPath]
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)] //, AllowsBackgroundLoading = true
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(ToolWindowHost))]
    [Guid(PackageGuidString)]
    [ProvideOptionPage(typeof(Options), "AnyStatus", "General", 0, 0, true)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    public sealed class AnyStatusPackage : Package
    {
        public const string PackageGuidString = "b8682407-a118-4468-9c71-d1f7c6a312ec";

        public const int ToolbarId = 0x1000;

        protected override void Initialize()
        {
            base.Initialize();

            try
            {
                var container = new ContainerBuilder(this).Build();

                var toolWindowCommand = container.Resolve<ToolWindowCommand>();

                var registry = container.Resolve<ItemRegistry>();

                toolWindowCommand.Initialize();

                JobManager.Initialize(registry);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}
