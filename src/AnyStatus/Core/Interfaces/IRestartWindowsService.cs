using System.Threading.Tasks;

namespace AnyStatus
{
    public interface IRestartWindowsService<in T> : IHandler where T : Item
    {
        Task HandleAsync(T item);
    }
}
