using AnyStatus.Interfaces;
using AnyStatus.Views;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace AnyStatus.VSPackage
{
    [ProvideBindingPath]
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)] //, AllowsBackgroundLoading = true
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(ToolWindowHost))]
    [Guid(PackageGuids.guidToolWindowPackageString)]
    [ProvideOptionPage(typeof(Options), "AnyStatus", "General", 0, 0, true)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    public sealed class AnyStatusPackage : Package
    {
        protected override void Initialize()
        {
            base.Initialize();

            ILogger logger = null;

            try
            {
                var container = ContainerBuilder.Build(this);

                var commands = container.ResolveAll<IMenuCommand>();

                AddCommands(commands);

                System.Threading.Tasks.Task.Run(() => 
                {
                    var userSettings = container.Resolve<IUserSettings>();

                    userSettings.Initialize();

                    logger = container.Resolve<ILogger>();
                    logger.IsEnabled = userSettings.DebugMode;
                    logger.Info("Initializing...");
                    logger.Info($"Client Id: {userSettings.ClientId}");

                    var usageReporter = container.Resolve<IUsageReporter>();
                    usageReporter.ClientId = userSettings.ClientId;
                    usageReporter.IsEnabled = userSettings.ReportAnonymousUsage;

                    container.Resolve<IJobScheduler>().Initialize();
                });
            }
            catch (Exception ex)
            {
                logger?.Error(ex, "Initialization failed.");
            }
        }

        private void AddCommands(IEnumerable<IMenuCommand> commands)
        {
            var commandService = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;

            foreach (var command in commands)
            {
                commandService.AddCommand(command.MenuCommand);
            }
        }
    }
}
