using AnyStatus.Interfaces;
using AnyStatus.Views;
using FluentScheduler;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;

namespace AnyStatus.VSPackage
{
    internal sealed class ToolWindowCommand
    {
        private readonly ILogger _logger;
        private readonly Package _package;
        private readonly IUserSettings _userSettings;
        private readonly IServiceProvider _serviceProvider;

        public ToolWindowCommand(Package package, ILogger logger, IUserSettings userSettings)
        {
            if (package == null)
                throw new ArgumentNullException(nameof(package));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (userSettings == null)
                throw new ArgumentNullException(nameof(userSettings));

            _userSettings = userSettings;
            _logger = logger;
            _package = package;
            _serviceProvider = package as IServiceProvider;
        }

        public void Initialize()
        {
            _logger.IsEnabled = _userSettings.DebugMode;

            AddCommands();
        }
        
        private void AddCommands()
        {
            var commandService = _serviceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;

            if (commandService == null)
                return;

            //view -> other windows 
            var menuCommandID = new CommandID(PackageGuids.guidToolWindowPackageCmdSet, PackageIds.ToolWindowCommandId);
            var menuItem = new MenuCommand(ShowToolWindow, menuCommandID);
            commandService.AddCommand(menuItem);

            //refresh toolbar command
            var refreshToolbarCommandId = new CommandID(PackageGuids.guidToolWindowPackageCmdSet, PackageIds.refreshToolbarCommandId);
            var refreshToolbarMenuItem = new MenuCommand(new EventHandler(RefreshButtonHandler), refreshToolbarCommandId);
            commandService.AddCommand(refreshToolbarMenuItem);

            //collapse toolbar command
            var collapseToolbarCommandId = new CommandID(PackageGuids.guidToolWindowPackageCmdSet, PackageIds.collapseToolbarCommandId);
            var collapseToolbarMenuItem = new MenuCommand(new EventHandler(CollapseAllButtonHandler), collapseToolbarCommandId);
            commandService.AddCommand(collapseToolbarMenuItem);

            //expand toolbar command
            var expandToolbarCommandId = new CommandID(PackageGuids.guidToolWindowPackageCmdSet, PackageIds.expandToolbarCommandId);
            var expandToolbarMenuItem = new MenuCommand(new EventHandler(ExpandAllButtonHandler), expandToolbarCommandId);
            commandService.AddCommand(expandToolbarMenuItem);

            //options toolbar command
            var optionsToolbarCommandId = new CommandID(PackageGuids.guidToolWindowPackageCmdSet, PackageIds.optionsToolbarCommandId);
            var optionsToolbarMenuItem = new MenuCommand(new EventHandler(OptionsButtonHandler), optionsToolbarCommandId);
            commandService.AddCommand(optionsToolbarMenuItem);

            //help toolbar command
            var helpToolbarCommandId = new CommandID(PackageGuids.guidToolWindowPackageCmdSet, PackageIds.helpToolbarCommandId);
            var helpToolbarMenuItem = new MenuCommand(new EventHandler(HelpButtonHandler), helpToolbarCommandId);
            commandService.AddCommand(helpToolbarMenuItem);
        }

        private void ExpandAllButtonHandler(object sender, EventArgs e)
        {
            _userSettings.RootItem?.ExpandAll();
        }

        private void CollapseAllButtonHandler(object sender, EventArgs e)
        {
            _userSettings.RootItem?.CollapseAll();
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
                _logger.Error(ex, "Failed to open options dialog.");
            }
        }

        private void RefreshButtonHandler(object sender, EventArgs e)
        {
            try
            {
                _logger.Info("Refreshing all items.");

                foreach (var schedule in JobManager.AllSchedules)
                {
                    schedule.Execute();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to refresh all items.");
            }
        }

        private void HelpButtonHandler(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("https://github.com/AlonAm/AnyStatus/wiki");
            }
            catch
            {
                // Ignore
            }
        }
    }
}
