namespace AnyStatus.VSPackage
{
    public class InfoBarService : IInfoBarService
    {
        private readonly IPackage _package;

        public InfoBarService(IPackage package)
        {
            _package = package;
        }

        public void ShowInfoBar()
        {
            var toolWindow = _package.FindToolWindow();

            toolWindow.AddInfoBar();
        }
    }
}
