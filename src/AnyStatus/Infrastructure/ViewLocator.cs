using AnyStatus.Interfaces;
using AnyStatus.Models;
using AnyStatus.ViewModels;
using AnyStatus.Views;

namespace AnyStatus.Infrastructure
{
    public class ViewLocator : IViewLocator
    {
        private TinyIoCContainer _container;

        public ViewLocator(TinyIoCContainer container)
        {
            _container = container;
        }

        public NewStatusDialog NewStatusDialog(Item parent)
        {
            var view = _container.Resolve<NewStatusDialog>();
            var viewModel = _container.Resolve<NewStatusViewModel>();

            viewModel.CloseRequested += (s, e) => { view.Close(); };

            viewModel.Parent = parent;
            view.DataContext = viewModel;

            return view;
        }
    }
}
