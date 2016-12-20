using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AnyStatus.Tests
{
    [TestClass]
    public class SetupAssemblyInitializer
    {
        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            State.SetMetadata(Theme.Default.Metadata);

            TinyIoCContainer.Current.RegisterMultiple(typeof(Item), new[] { typeof(Dummy) });

            TinyIoCContainer.Current.Register(typeof(IMonitor<Dummy>), typeof(DummyMonitor)).AsMultiInstance();
        }
    }
}
