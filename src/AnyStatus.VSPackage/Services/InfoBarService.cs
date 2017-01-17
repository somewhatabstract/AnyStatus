using Microsoft.VisualStudio.Shell;

namespace AnyStatus.VSPackage
{
    public class InfoBarService : IInfoBarService
    {
        private bool _isActive;
        private ToolWindowPane _toolWindow;

        private readonly ILogger _logger;
        private readonly IPackage _package;
        private readonly ISettingsStore _settingsStore;

        public InfoBarService(IPackage package, ILogger logger, ISettingsStore settingsStore)
        {
            _package = Preconditions.CheckNotNull(package, nameof(package));
            _logger = Preconditions.CheckNotNull(logger, nameof(logger));
            _settingsStore = Preconditions.CheckNotNull(settingsStore, nameof(settingsStore));
        }

        public void ShowSettingsChangedInfoBar()
        {
            if (_isActive)
                return;

            ThreadHelper.JoinableTaskFactory.Run(async delegate
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                AddInfoBar(new ConfigurationChangedInfoBar());

                _isActive = true;
            });
        }

        private void AddInfoBar(ConfigurationChangedInfoBar infoBar)
        {
            var toolWindow = GetToolWindow();

            toolWindow.InfoBarClosed += OnInfoBarClosed;

            toolWindow.InfoBarActionItemClicked += OnInfoBarActionItemClicked;

            toolWindow.AddInfoBar(infoBar);
        }

        private void OnInfoBarActionItemClicked(object sender, InfoBarActionItemEventArgs e)
        {
            if (e.ActionItem.ActionContext == ConfigurationChangedInfoBar.ReloadButtonContext)
            {
                _logger.Info("Reloading configuration settings.");

                _settingsStore.TryReload();
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
