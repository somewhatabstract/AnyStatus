using AnyStatus.Interfaces;
using AnyStatus.ViewModels;
using Microsoft.VisualStudio.Shell;
using System.Windows;
using System.ComponentModel;

#warning Options and ToolWindow are using different instances of UserSettings

namespace AnyStatus.Views
{
    public class Options : UIElementDialogPage
    {
        private ILogger _logger;
        private IUserSettings _userSettings;
        private OptionsViewModel _viewModel;
        private OptionsDialogControl _optionsDialog;

        public Options()
        {
            _logger = new NullLogger();
            _userSettings = new UserSettings(_logger);
            _viewModel = new OptionsViewModel(_userSettings);
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