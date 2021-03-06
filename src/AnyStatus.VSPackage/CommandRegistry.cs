﻿using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;

namespace AnyStatus.VSPackage
{
    public class CommandRegistry : ICommandRegistry
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IEnumerable<IMenuCommand> _commands;

        public CommandRegistry(IServiceProvider serviceProvider, IEnumerable<IMenuCommand> commands)
        {
            _commands = Preconditions.CheckNotNull(commands, nameof(commands));
            _serviceProvider = Preconditions.CheckNotNull(serviceProvider, nameof(serviceProvider)); ;
        }

        public void RegisterCommands()
        {
            var commandService = _serviceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;

            foreach (var command in _commands)
            {
                commandService.AddCommand(command.MenuCommand);
            }
        }
    }
}
