//------------------------------------------------------------------------------
// <copyright file="ToolWindowPackage.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using FluentScheduler;
using AnyStatus.Interfaces;
using AnyStatus.Views;
using Microsoft.VisualStudio.Shell;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Runtime.InteropServices;
using TinyIoC;
using AnyStatus.Infrastructure;

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
    public sealed class VSPackage : Package
    {
        public const string PackageGuidString = "b8682407-a118-4468-9c71-d1f7c6a312ec";

        public const int ToolbarId = 0x1000;

        public VSPackage()
        {
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.
        }

        protected override void Initialize()
        {
            base.Initialize();

            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true; //ignore ssl errors

            TinyIoCContainer container = new ContainerBuilder().Build(this);

            container.Resolve<ToolWindowCommand>().Initialize();

            JobManager.Initialize(container.Resolve<JobRegistry>());

            container.Resolve<ILogger>().Log("AnyStatus started.");
        }

        private static void InitializeJobManager()
        {
            

            //var registry = new Registry();
            //registry.Schedule(() =>
            //{
            //    if (outputWindowPane != null)
            //    {
            //        outputWindowPane.OutputString($"Thread: {System.Threading.Thread.CurrentThread.ManagedThreadId}\r\n");
            //    }
            //}).ToRunNow().AndEvery(2).Seconds();
            //JobManager.Initialize(registry);
            ////////////////////////////////
        }
    }
}
