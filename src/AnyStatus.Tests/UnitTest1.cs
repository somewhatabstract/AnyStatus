using AnyStatus.Infrastructure;
using AnyStatus.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace AnyStatus.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var baseHandlerType = typeof(IHandler<>);

            var handlerTypes =
                    from assembly in new[] { typeof(Item).Assembly }
                    from type in assembly.GetTypes()
                    where !type.IsAbstract && !type.IsGenericTypeDefinition
                    let handlerInterfaces =
                        from iface in type.GetInterfaces()
                        where iface.IsGenericType
                        where iface.GetGenericTypeDefinition() == baseHandlerType
                        select iface
                    where handlerInterfaces.Any()
                    select type;


            foreach (var handlerType in handlerTypes)
            {
                TinyIoCContainer.Current.Register(handlerType.GetInterface(baseHandlerType.Name), handlerType).AsMultiInstance();
            }

            var handler = TinyIoCContainer.Current.Resolve<IHandler<WindowsService>>();

            Assert.IsNotNull(handler);
            Assert.IsInstanceOfType(handler, typeof(WindowsServiceHandler));
        }
    }
}
