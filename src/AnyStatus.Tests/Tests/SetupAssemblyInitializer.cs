using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AnyStatus.Tests.Tests
{
    [TestClass]
    public class SetupAssemblyInitializer
    {
        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            TinyIoCContainer.Current.RegisterMultiple(typeof(Item), new[] { typeof(Dummy) });

            TinyIoCContainer.Current.Register(typeof(IHandler<Dummy>), typeof(DummyHandler)).AsMultiInstance();
        }
    }
}
