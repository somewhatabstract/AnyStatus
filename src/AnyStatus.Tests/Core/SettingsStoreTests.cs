using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.IO;
using System.Threading.Tasks;

namespace AnyStatus.Tests
{
    [TestClass]
    public class SettingsStoreTests
    {
        private static TestContext _testContext;

        [ClassInitialize]
        public static void SetupTests(TestContext testContext)
        {
            _testContext = testContext;
        }

        [TestMethod]
        public void SettingsStore_TryInitialize()
        {
            var logger = Substitute.For<ILogger>();

            var store = new SettingsStore(logger);

            var result = store.TryInitialize();

            Assert.IsTrue(result);
            Assert.IsNotNull(store.Settings);
        }

        [TestMethod]
        public async Task SettingsStore_TryInitializeAsync()
        {
            var logger = Substitute.For<ILogger>();

            var store = new SettingsStore(logger);

            var result = await store.TryInitializeAsync();

            Assert.IsTrue(result);
            Assert.IsNotNull(store.Settings);
        }


        [TestMethod]
        public void SettingsStore_TrySave()
        {
            var logger = Substitute.For<ILogger>();

            var store = new SettingsStore(logger);

            store.TryInitialize();

            var result = store.TrySave();

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void SettingsStore_TryRestoreDefaultSettings()
        {
            var logger = Substitute.For<ILogger>();

            var store = new SettingsStore(logger);

            var result = store.TryRestoreDefaultSettings();

            Assert.IsTrue(result);
            Assert.IsNotNull(store.Settings);
        }

        [TestMethod]
        public void SettingsStore_TryImport()
        {
            var filePath = Path.Combine(_testContext.TestRunDirectory, "ImportTest.xml");

            var logger = Substitute.For<ILogger>();

            var store = new SettingsStore(logger);

            store.TryInitialize();

            store.TryExport(filePath);

            var result = store.TryImport(filePath);

            Assert.IsTrue(result);
            Assert.IsNotNull(store.Settings);
        }

        [TestMethod]
        public void SettingsStore_TryExport()
        {
            var filePath = Path.Combine(_testContext.TestRunDirectory, "ExportTest.xml");

            var logger = Substitute.For<ILogger>();

            var store = new SettingsStore(logger);

            store.TryInitialize();

            var result = store.TryExport(filePath);

            Assert.IsTrue(result);
            Assert.IsTrue(File.Exists(filePath));
        }

        [TestMethod]
        public void UserSettings_Create()
        {
            var settings = UserSettings.Create();

            Assert.IsNotNull(settings.ClientId);
            Assert.IsNotNull(settings.RootItem);
            Assert.IsNotNull(settings.CustomTheme);

            Assert.IsTrue(settings.ShowStatusColors);
            Assert.IsTrue(settings.ShowStatusIcons);
            Assert.IsTrue(settings.DebugMode);
            Assert.IsTrue(settings.ReportAnonymousUsage);
        }
    }
}
