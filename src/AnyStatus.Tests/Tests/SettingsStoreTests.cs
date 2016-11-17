using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.IO;
using System.Threading.Tasks;

namespace AnyStatus.Tests
{
    [TestClass]
    public class SettingsStoreTests
    {
        public TestContext TestContext { get; set; }

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
            var filePath = Path.Combine(TestContext.TestRunDirectory, "ImportTest.xml");

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
            var filePath = Path.Combine(TestContext.TestRunDirectory, "ExportTest.xml");

            var logger = Substitute.For<ILogger>();

            var store = new SettingsStore(logger);

            store.TryInitialize();

            var result = store.TryExport(filePath);

            Assert.IsTrue(result);
            Assert.IsTrue(File.Exists(filePath));
        }

        [TestMethod]
        public void AppSettings_Create()
        {
            var settings = AppSettings.Default();

            Assert.IsNotNull(settings.ClientId);
            Assert.IsNotNull(settings.RootItem);
            Assert.IsNotNull(settings.Theme);

            Assert.IsTrue(settings.ShowStatusColors);
            Assert.IsTrue(settings.ShowStatusIcons);
            Assert.IsTrue(settings.DebugMode);
            Assert.IsTrue(settings.ReportAnonymousUsage);
        }
    }
}
