namespace AnyStatus
{
    public interface ITriggerBuild<in T> : IHandler where T : Item
    {
        void Handle(T item);
    }
}
