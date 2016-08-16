using AnyStatus.Views;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;

namespace AnyStatus.VSPackage
{
    internal sealed class ToolWindowCommand
    {
        public const int CommandId = 0x0100;
        public static readonly Guid CommandSet = new Guid("c420854f-cac2-4492-8067-ecf632228390");

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
                var menuCommandID = new CommandID(CommandSet, CommandId);
                var menuItem = new MenuCommand(ShowToolWindow, menuCommandID);
                commandService.AddCommand(menuItem);

                //options toolbar command
                var optionsToolbarCommandId = new CommandID(CommandSet, 0x1002);
                var optionsToolbarMenuItem = new MenuCommand(new EventHandler(OptionsButtonHandler), optionsToolbarCommandId);
                commandService.AddCommand(optionsToolbarMenuItem);

                //refresh toolbar command
                //var refreshToolbarCommandId = new CommandID(CommandSet, 0x1003);
                //var refreshToolbarMenuItem = new MenuCommand(new EventHandler(RefreshButtonHandler), refreshToolbarCommandId);
                //commandService.AddCommand(refreshToolbarMenuItem);
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
            _package.ShowOptionPage(typeof(Options));
        }

        //private void RefreshButtonHandler(object sender, EventArgs e)
        //{
        //    MessageBox.Show("Not Implemented");
        //}
    }
}
