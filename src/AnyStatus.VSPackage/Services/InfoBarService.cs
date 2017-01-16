using Microsoft.VisualStudio.Shell;
using System;

namespace AnyStatus.VSPackage
{
    public class InfoBarService : IInfoBarService
    {
        private bool _isActive;
        private readonly ILogger _logger;
        private readonly IPackage _package;
        private ToolWindowPane _toolWindow;

        public InfoBarService(IPackage package, ILogger logger)
        {
            _package = Preconditions.CheckNotNull(package, nameof(package));
            _logger = Preconditions.CheckNotNull(logger, nameof(logger));
        }

        public void ShowSettingsChangedInfoBar()
        {
            if (_isActive)
                return;

            var toolWindow = GetToolWindow();

            ThreadHelper.JoinableTaskFactory.Run(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                toolWindow.AddInfoBar(new ConfigurationChangedInfoBar());

                toolWindow.InfoBarActionItemClicked += OnInfoBarActionItemClicked;

                toolWindow.InfoBarClosed += OnInfoBarClosed;

                _isActive = true;
            });
        }

        private void OnInfoBarActionItemClicked(object sender, InfoBarActionItemEventArgs e)
        {
            if (e.ActionItem.Text == "Reload")
            {
                _logger.Info("Reloading configuration settings.");

                var settingsStore = TinyIoCContainer.Current.Resolve<ISettingsStore>();

                //settingsStore.TryReload();
            }

            var toolWindow = GetToolWindow();

            toolWindow.RemoveInfoBar(e.InfoBarModel);
        }

        private void OnInfoBarClosed(object sender, InfoBarEventArgs e)
        {
            var toolWindow = GetToolWindow();

            toolWindow.InfoBarActionItemClicked -= OnInfoBarActionItemClicked;

            toolWindow.InfoBarClosed -= OnInfoBarClosed;

            _isActive = false;
        }

        private ToolWindowPane GetToolWindow()
        {
            if (_toolWindow == null)
            {
                _toolWindow = (ToolWindowPane)_package.FindToolWindow();
            }

            return _toolWindow;
        }
    }
}
