using AnyStatus.Features.Edit;
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

        public NewWindow NewWindow(Item parent)
        {
            var view = _container.Resolve<NewWindow>();
            var viewModel = _container.Resolve<NewViewModel>();

            viewModel.CloseRequested += (s, e) => { view.Close(); };

            viewModel.Parent = parent;
            view.DataContext = viewModel;

            return view;
        }

        public EditWindow EditWindow(Item item)
        {
            var view = _container.Resolve<EditWindow>();
            var viewModel = _container.Resolve<EditViewModel>();

            viewModel.Item = item;
            viewModel.CloseRequested += (s, e) => { view.Close(); };
            
            view.DataContext = viewModel;

            return view;
        }

        //public OptionsDialogControl OptionsWindow()
        //{
        //    return _container.Resolve<OptionsDialogControl>();
        //}
    }
}
