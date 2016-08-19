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

        public NewItemDialog NewStatusDialog(Item parent)
        {
            var view = _container.Resolve<NewItemDialog>();
            var viewModel = _container.Resolve<NewItemViewModel>();

            viewModel.CloseRequested += (s, e) => { view.Close(); };

            viewModel.Parent = parent;
            view.DataContext = viewModel;

            return view;
        }
    }
}
