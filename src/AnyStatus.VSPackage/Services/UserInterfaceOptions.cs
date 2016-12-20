using Microsoft.VisualStudio.Shell;
using System.ComponentModel;
using System.Windows;

namespace AnyStatus.VSPackage
{
    public class UserInterfaceOptions : UIElementDialogPage
    {
        private UserInterfaceOptionsView _view;
        private UserInterfaceOptionsViewModel _viewModel;

        public UserInterfaceOptions() : this(TinyIoCContainer.Current.Resolve<UserInterfaceOptionsViewModel>()) { }

        public UserInterfaceOptions(UserInterfaceOptionsViewModel viewModel)
        {
            _viewModel = Preconditions.CheckNotNull(viewModel, nameof(viewModel));

            _view = new UserInterfaceOptionsView(viewModel);
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