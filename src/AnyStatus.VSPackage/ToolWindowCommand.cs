using AnyStatus.Views;
using FluentScheduler;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Windows;

namespace AnyStatus.VSPackage
{
    internal sealed class ToolWindowCommand
    {
        private readonly Package _package;
        private readonly IServiceProvider _serviceProvider;

        public ToolWindowCommand(Package package)
        {
            if (package == null)
                throw new ArgumentNullException(nameof(package));

            _package = package;
            _serviceProvider = package;
        }

        public void Initialize()
        {
            OleMenuCommandService commandService = _serviceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;

            if (commandService != null)
            {
                //view -> other windows 
                var menuCommandID = new CommandID(PackageGuids.guidToolWindowPackageCmdSet, PackageIds.ToolWindowCommandId);
                var menuItem = new MenuCommand(ShowToolWindow, menuCommandID);
                commandService.AddCommand(menuItem);

                //refresh toolbar command
                var refreshToolbarCommandId = new CommandID(PackageGuids.guidToolWindowPackageCmdSet, PackageIds.refreshToolbarCommandId);
                var refreshToolbarMenuItem = new MenuCommand(new EventHandler(RefreshButtonHandler), refreshToolbarCommandId);
                commandService.AddCommand(refreshToolbarMenuItem);

                //options toolbar command
                var optionsToolbarCommandId = new CommandID(PackageGuids.guidToolWindowPackageCmdSet, PackageIds.optionsToolbarCommandId);
                var optionsToolbarMenuItem = new MenuCommand(new EventHandler(OptionsButtonHandler), optionsToolbarCommandId);
                commandService.AddCommand(optionsToolbarMenuItem);

                //help toolbar command
                var helpToolbarCommandId = new CommandID(PackageGuids.guidToolWindowPackageCmdSet, PackageIds.helpToolbarCommandId);
                var helpToolbarMenuItem = new MenuCommand(new EventHandler(HelpButtonHandler), helpToolbarCommandId);
                commandService.AddCommand(helpToolbarMenuItem);
            }
        }

        

        private void ShowToolWindow(object sender, EventArgs e)
        {
            ToolWindowPane window = _package.FindToolWindow(typeof(ToolWindowHost), 0, true);
            
            if ((null == window) || (null == window.Frame))
            {
                throw new NotSupportedException("Cannot create tool window");
            }

            IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;

            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }

        private void OptionsButtonHandler(object sender, EventArgs e)
        {
            try
            {
                _package.ShowOptionPage(typeof(Options));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private void RefreshButtonHandler(object sender, EventArgs e)
        {
            try
            {
                foreach (var schedule in JobManager.AllSchedules)
                {
                    schedule.Execute();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private void HelpButtonHandler(object sender, EventArgs e)
        {
            Process.Start("https://github.com/AlonAm/AnyStatus/wiki");
        }
    }
}
