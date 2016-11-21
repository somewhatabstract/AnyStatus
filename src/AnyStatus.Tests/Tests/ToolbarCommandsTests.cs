using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Microsoft.VisualStudio.Shell;

namespace AnyStatus.Tests
{
    [TestClass]
    public class ToolbarCommandsTests
    {
        [TestMethod]
        public void ExpandAllCommand_Should_Expand_All_Items_In_Settings()
        {
            var item = new Item { IsExpanded = false };
            var settings = new UserSettings { RootItem = new RootItem() };
            var settingsStore = Substitute.For<ISettingsStore>();

            settings.RootItem.Add(item);

            settingsStore.Settings.Returns(settings);

            var command = new ExpandAllCommand(settingsStore);

            command.MenuCommand.Invoke();

            Assert.IsTrue(item.IsExpanded);
        }

        [TestMethod]
        public void CollapseAllCommand_Should_Collapse_All_Items_In_Settings()
        {
            var item = new Item { IsExpanded = true };
            var settings = new UserSettings { RootItem = new RootItem() };
            var settingsStore = Substitute.For<ISettingsStore>();

            settings.RootItem.Add(item);

            settingsStore.Settings.Returns(settings);

            var command = new CollapseAllCommand(settingsStore);

            command.MenuCommand.Invoke();

            Assert.IsFalse(item.IsExpanded);
        }

        [TestMethod]
        public void OptionsCommand_Should_Show_General_Options_Page()
        {
            var logger = Substitute.For<ILogger>();
            var package = Substitute.For<Package>();
            var command = new OptionsCommand(package, logger);

            command.MenuCommand.Invoke();

            package.Received(1).ShowOptionPage(typeof(GeneralOptions));
        }

        [TestMethod]
        public void RefreshAllCommand_Should_Execute_All_Scheduled_Jobs()
        {
            var logger = Substitute.For<ILogger>();
            var jobScheduler = Substitute.For<IJobScheduler>();
            var command = new RefreshAllCommand(jobScheduler, logger);

            command.MenuCommand.Invoke();

            jobScheduler.Received(1).ExecuteAll();
        }
    }
}
