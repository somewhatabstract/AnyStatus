using AnyStatus.Infrastructure;
using AnyStatus.Interfaces;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Configuration;
using System.Diagnostics;
using Task = System.Threading.Tasks.Task;

namespace AnyStatus.VSPackage
{
    public class AnyStatusApp
    {
        ILogger _logger;
        IUserSettings _userSettings;
        IUsageReporter _usageReporter;
        IJobScheduler _jobScheduler;
        IServiceProvider _serviceProvider;
        IEnumerable<IMenuCommand> _commands;

        public AnyStatusApp(ILogger logger,
                            IUserSettings userSettings,
                            IUsageReporter usageReporter,
                            IJobScheduler jobScheduler,
                            IEnumerable<IMenuCommand> commands,
                            IServiceProvider serviceProvider)
        {
            _logger = Preconditions.CheckNotNull(logger, nameof(logger));
            _userSettings = Preconditions.CheckNotNull(userSettings, nameof(userSettings));
            _usageReporter = Preconditions.CheckNotNull(usageReporter, nameof(usageReporter));
            _jobScheduler = Preconditions.CheckNotNull(jobScheduler, nameof(jobScheduler));
            _commands = Preconditions.CheckNotNull(commands, nameof(commands));
            _serviceProvider = Preconditions.CheckNotNull(serviceProvider, nameof(serviceProvider));
        }

        public async Task InitializeAsync()
        {
            try
            {
                _logger.Info("Initializing...");

                LogConfigurationFilePath();

                AddCommands(_commands);

                await Task.Run(() => Initialize());

                _usageReporter.ReportStartSession();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Initialization failed.");
            }
        }

        private void Initialize()
        {
            _userSettings.Initialize();

            _logger.IsEnabled = _userSettings.DebugMode;

            _usageReporter.ClientId = _userSettings.ClientId;

            _usageReporter.IsEnabled = _userSettings.ReportAnonymousUsage;

            _jobScheduler.Initialize();
        }

        private void AddCommands(IEnumerable<IMenuCommand> commands)
        {
            var commandService = _serviceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;

            foreach (var command in commands)
            {
                commandService.AddCommand(command.MenuCommand);
            }
        }

        [Conditional("DEBUG")]
        private void LogConfigurationFilePath()
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);

            _logger.Info("Configuration File Path: " + config.FilePath);
        }
    }
}
