using AnyStatus.Infrastructure;
using AnyStatus.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.ObjectModel;
using System.Linq;

namespace AnyStatus.Tests
{
    [TestClass]
    public class ItemTests
    {


        [TestMethod]
        public void IsParentOf()
        {
            var item = new Item();
            var child = new Item();

            item.Items = new ObservableCollection<Item>()
            {
                child
            };

            Assert.IsTrue(item.IsParentOf(child));
        }

        [TestMethod]
        public void MoveTo()
        {
            var item = new Item();
        }

        [TestMethod]
        public void Add()
        {
            var parent = new Item();
            var item = new Item();

            parent.Add(item);

            Assert.IsTrue(parent.Items.Contains(item));
        }

        [TestMethod]
        public void Delete()
        {
            var parent = new Item();
            var item = new Item();

            parent.Add(item);

            item.Delete();

            Assert.IsFalse(parent.Items.Contains(item));
        }

        [TestMethod]
        public void MoveUp()
        {
            var parent = new Item();

            var item1 = new Item();
            var item2 = new Item();

            parent.Add(item1);
            parent.Add(item2);

            item2.MoveUp();

            Assert.IsTrue(parent.Items.IndexOf(item2) == 0);
        }

        [TestMethod]
        public void MoveDown()
        {
            var parent = new Item();

            var item1 = new Item();
            var item2 = new Item();

            parent.Add(item1);
            parent.Add(item2);

            item1.MoveDown();

            Assert.IsTrue(parent.Items.IndexOf(item1) == 1);
        }
    }
}

//[Ignore]
//[TestMethod]
//public void ObjectDiscoveryTest()
//{
//    var baseHandlerType = typeof(IHandler<>);

//    var handlerTypes =
//            from assembly in new[] { typeof(Item).Assembly }
//            from type in assembly.GetTypes()
//            where !type.IsAbstract && !type.IsGenericTypeDefinition
//            let handlerInterfaces =
//                from iface in type.GetInterfaces()
//                where iface.IsGenericType
//                where iface.GetGenericTypeDefinition() == baseHandlerType
//                select iface
//            where handlerInterfaces.Any()
//            select type;

//    foreach (var handlerType in handlerTypes)
//    {
//        TinyIoCContainer.Current.Register(handlerType.GetInterface(baseHandlerType.Name), handlerType).AsMultiInstance();
//    }

//    var handler = TinyIoCContainer.Current.Resolve<IHandler<WindowsService>>();

//    Assert.IsNotNull(handler);
//    Assert.IsInstanceOfType(handler, typeof(WindowsServiceHandler));
//}
