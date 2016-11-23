using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Linq;
using System.Windows.Input;

namespace AnyStatus.Tests.Tests
{
    [TestClass]
    public class ToolWindowViewModelTests
    {
        [TestMethod]
        public void Initialize_Should_Set_All_Properties()
        {
            var settings = new UserSettings();
            var logger = Substitute.For<ILogger>();
            var viewLocator = Substitute.For<IViewLocator>();
            var jobScheduler = Substitute.For<IJobScheduler>();
            var settingsStore = Substitute.For<ISettingsStore>();
            var mediator = Substitute.For<IMediator>();

            settingsStore.Settings.Returns(settings);

            var viewModel = new ToolWindowViewModel(jobScheduler, settingsStore, viewLocator, mediator, logger);

            var anyPropertyIsNull = viewModel.GetType().GetProperties()
                                             .Select(pi => pi.GetValue(viewModel))
                                             .Any(value => value == null);

            Assert.IsFalse(anyPropertyIsNull);
        }
    }
}
