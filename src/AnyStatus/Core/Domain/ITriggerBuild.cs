namespace AnyStatus
{
    public interface ITriggerBuild<in T> where T : Item
    {
        void Handle(T item);
    }
}
