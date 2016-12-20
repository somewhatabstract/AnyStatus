namespace AnyStatus
{
    public interface IOpenInBrowser<in T> : IHandler where T : Item
    {
        void Handle(T item);
    }
}