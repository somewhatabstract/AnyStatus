using AnyStatus.Infrastructure;
using AnyStatus.ViewModels;
using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel;
using System.Windows;

namespace AnyStatus.Views
{
    public class Options : UIElementDialogPage
    {
        private OptionsViewModel _viewModel;
        private OptionsDialogControl _optionsDialog;

        public Options() : this(TinyIoCContainer.Current.Resolve<OptionsViewModel>())
        {
        }

        public Options(OptionsViewModel viewModel)
        {
            if (viewModel == null)
                throw new ArgumentNullException(nameof(viewModel));

            _viewModel = viewModel;
            _optionsDialog = new OptionsDialogControl(viewModel);
        }

        protected override UIElement Child
        {
            get { return _optionsDialog; }
        }

        protected override void OnActivate(CancelEventArgs e)
        {
            base.OnActivate(e);

            if (_viewModel.ActivateCommand.CanExecute(null))
                _viewModel.ActivateCommand.Execute(null);
        }

        protected override void OnApply(PageApplyEventArgs e)
        {
            if (e.ApplyBehavior == ApplyKind.Apply && _viewModel.ApplyCommand.CanExecute(null))
                _viewModel.ApplyCommand.Execute(null);

            base.OnApply(e);
        }
    }
}