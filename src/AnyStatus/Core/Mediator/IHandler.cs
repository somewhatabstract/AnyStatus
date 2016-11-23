namespace AnyStatus
{
    public interface IHandler<in T>
    {
        void Handle(T item);
    }
}