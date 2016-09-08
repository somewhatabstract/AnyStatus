using AnyStatus.Models;

namespace AnyStatus
{
    public interface IHandler<in T> where T : Item
    {
        void Handle(T item);
    }
}