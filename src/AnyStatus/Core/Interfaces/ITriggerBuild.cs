using System.Threading.Tasks;

namespace AnyStatus
{
    public interface ITriggerBuild<in T> : IHandler where T : Item
    {
        Task HandleAsync(T item);
    }
}
