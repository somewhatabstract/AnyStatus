using FluentScheduler;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace AnyStatus.Tests
{
    [TestClass]
    public class AnyStatusAppTests
    {
        ILogger _logger = Substitute.For<ILogger>();
        ISettingsStore _settingsStore = Substitute.For<ISettingsStore>();
        IUsageReporter _usageReporter = Substitute.For<IUsageReporter>();
        IJobScheduler _jobScheduler = Substitute.For<IJobScheduler>();
        ICommandRegistry _commandRegistry = Substitute.For<ICommandRegistry>();
        private Dummy _item = new Dummy { Id = Guid.NewGuid(), Name = "Test" };

        public AnyStatusAppTests()
        {
            var settings = new UserSettings
            {
                RootItem = new RootItem
                {
                    Items = new ObservableCollection<Item> { _item }
                }
            };

            _settingsStore.Settings.Returns(settings);
        }
        [TestMethod]
        public async Task InitializeAsync()
        {
            var app = new AnyStatusApp(_logger, _settingsStore, _usageReporter, _jobScheduler, _commandRegistry);

            await app.InitializeAsync();

            _commandRegistry.Received(1).RegisterCommands();

            await _settingsStore.Received(1).TryInitializeAsync();
        }

        [TestMethod]
        public async Task Start()
        {
            var app = new AnyStatusApp(_logger, _settingsStore, _usageReporter, _jobScheduler, _commandRegistry);

            await app.InitializeAsync();

            app.Start();

            _jobScheduler.Received(1).Start();
            _usageReporter.Received(1).ReportStartSession();
            _jobScheduler.Received(1).Schedule(_settingsStore.Settings.RootItem, true);
        }

        [TestMethod]
        public async Task Stop()
        {
            var app = new AnyStatusApp(_logger, _settingsStore, _usageReporter, _jobScheduler, _commandRegistry);

            await app.InitializeAsync();

            app.Stop();

            _jobScheduler.Received(1).Stop();
            _usageReporter.Received(1).ReportEndSession();
        }
    }
}
