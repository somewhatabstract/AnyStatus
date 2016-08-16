namespace AnyStatus.Models
{
    public interface IHandler<in T> where T : Item
    {
        void Handle(T item);
    }
}