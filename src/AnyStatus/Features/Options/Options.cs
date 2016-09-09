using AnyStatus.Infrastructure;
using AnyStatus.Interfaces;
using AnyStatus.ViewModels;
using Microsoft.VisualStudio.Shell;
using System.ComponentModel;
using System.Windows;

#warning Options and ToolWindow are using different instances of UserSettings

namespace AnyStatus.Views
{
    public class Options : UIElementDialogPage
    {
        private OptionsViewModel _viewModel;
        private OptionsDialogControl _optionsDialog;

        public Options()
        {
            var userSettings = TinyIoCContainer.Current.Resolve<IUserSettings>(); //todo: use MEF?

            _viewModel = new OptionsViewModel(userSettings);

            _optionsDialog = new OptionsDialogControl(_viewModel);
        }

        protected override UIElement Child
        {
            get { return _optionsDialog; }
        }

        protected override void OnActivate(CancelEventArgs e)
        {
            base.OnActivate(e);

            if (!_viewModel.ActivateCommand.CanExecute(null))
                return;

            _viewModel.ActivateCommand.Execute(null);
        }

        protected override void OnApply(PageApplyEventArgs e)
        {
            base.OnApply(e);

            if (e.ApplyBehavior != ApplyKind.Apply || !_viewModel.ApplyCommand.CanExecute(null))
                return;

            _viewModel.ApplyCommand.Execute(null);
        }
    }
}