namespace AnyStatus
{
    public interface IOpenInBrowser<in T> where T : Item
    {
        void Handle(T item);
    }
}