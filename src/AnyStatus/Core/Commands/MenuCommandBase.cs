using System;
using System.ComponentModel.Design;

namespace AnyStatus
{
    public abstract class ToolbarCommand : IMenuCommand
    {
        public ToolbarCommand(int commandId)
        {
            var command = new CommandID(PackageGuids.guidToolWindowPackageCmdSet, commandId);

            MenuCommand = new MenuCommand(new EventHandler(Handle), command);
        }

        protected abstract void Handle(object sender, EventArgs e);

        public MenuCommand MenuCommand { get; private set; }
    }
}
