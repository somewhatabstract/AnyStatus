namespace AnyStatus
{
    public interface IPackage
    {
        void ShowOptions();

        void ShowToolWindow();

        IToolWindow FindToolWindow();
    }
}
