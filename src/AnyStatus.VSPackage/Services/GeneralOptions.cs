using Microsoft.VisualStudio.Shell;
using System.ComponentModel;
using System.Windows;

namespace AnyStatus.VSPackage
{
    public class GeneralOptions : UIElementDialogPage
    {
        private readonly GeneralOptionsView _view;
        private readonly GeneralOptionsViewModel _viewModel;

        public GeneralOptions() : this(TinyIoCContainer.Current.Resolve<GeneralOptionsViewModel>()) { }

        public GeneralOptions(GeneralOptionsViewModel viewModel)
        {
            _viewModel = Preconditions.CheckNotNull(viewModel, nameof(viewModel));

            _view = new GeneralOptionsView(viewModel);
        }

        protected override UIElement Child
        {
            get { return _view; }
        }

        protected override void OnActivate(CancelEventArgs e)
        {
            base.OnActivate(e);

            if (_viewModel.ActivateCommand.CanExecute(null))
            {
                _viewModel.ActivateCommand.Execute(null);
            }
        }

        protected override void OnApply(PageApplyEventArgs e)
        {
            if (e.ApplyBehavior == ApplyKind.Apply && _viewModel.ApplyCommand.CanExecute(null))
            {
                _viewModel.ApplyCommand.Execute(null);
            }

            base.OnApply(e);
        }
    }
}