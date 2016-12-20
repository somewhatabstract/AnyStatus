namespace AnyStatus
{
    public interface IMonitor<in T> : IHandler where T : Item
    {
        void Handle(T item);
    }
}
