﻿using AnyStatus.Views;
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
    [ProvideOptionPage(typeof(Options), "AnyStatus", "General", 0, 0, true)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    public sealed class AnyStatusPackage : Package
    {
        protected override async void Initialize()
        {
            base.Initialize();

            try
            {
                var container = ContainerBuilder.Build(this);

                await container.Resolve<AnyStatusApp>().InitializeAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}
