using AnyStatus.Interfaces;
using AnyStatus.Views;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
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
    [Guid(PackageGuids.guidToolWindowPackageString)]
    [ProvideOptionPage(typeof(Options), "AnyStatus", "General", 0, 0, true)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    public sealed class AnyStatusPackage : Package
    {
        protected override void Initialize()
        {
            base.Initialize();

            try
            {
                var container = ContainerBuilder.Build(this);
                var commands = container.ResolveAll<IMenuCommand>();
                var userSettings = container.Resolve<IUserSettings>();
                var jobScheduler = container.Resolve<IJobScheduler>();
                var logger = container.Resolve<ILogger>();

                AddCommands(commands);
                userSettings.Initialize();
                logger.IsEnabled = userSettings.DebugMode;
                jobScheduler.Initialize();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private void AddCommands(IEnumerable<IMenuCommand> commands)
        {
            var commandService = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;

            if (commandService == null)
                return;

            //view -> other windows 
            var menuCommandID = new CommandID(PackageGuids.guidToolWindowPackageCmdSet, PackageIds.ToolWindowCommandId);
            var menuItem = new MenuCommand(ShowToolWindow, menuCommandID);
            commandService.AddCommand(menuItem);

            //registered commands
            foreach (var command in commands)
            {
                commandService.AddCommand(command.MenuCommand);
            }
        }

        private void ShowToolWindow(object sender, EventArgs e)
        {
            ToolWindowPane window = this.FindToolWindow(typeof(ToolWindowHost), 0, true);

            if ((null == window) || (null == window.Frame))
            {
                throw new NotSupportedException("Cannot create tool window");
            }

            IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;

            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }
    }
}
