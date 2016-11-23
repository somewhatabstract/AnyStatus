using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Linq;

namespace AnyStatus.Tests.Tests
{
    [TestClass]
    public class ToolWindowViewModelTests
    {
        [TestMethod]
        public void Initialize_Should_Set_All_Properties()
        {
            var mediator = Substitute.For<IMediator>();
            var settingsStore = Substitute.For<ISettingsStore>();

            settingsStore.Settings.Returns(new UserSettings());

            var viewModel = new ToolWindowViewModel(mediator, settingsStore);

            var anyPropertyIsNull = viewModel.GetType().GetProperties()
                                             .Select(pi => pi.GetValue(viewModel))
                                             .Any(value => value == null);

            Assert.IsFalse(anyPropertyIsNull);
        }
    }
}
