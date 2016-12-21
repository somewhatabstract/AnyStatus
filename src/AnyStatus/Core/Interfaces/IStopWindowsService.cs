using System.Threading.Tasks;

namespace AnyStatus
{
    public interface IStopWindowsService<in T> : IHandler where T : Item
    {
        Task HandleAsync(T item);
    }
}
