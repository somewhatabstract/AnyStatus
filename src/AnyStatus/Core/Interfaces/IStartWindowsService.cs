using System.Threading.Tasks;

namespace AnyStatus
{
    public interface IStartWindowsService<in T> : IHandler where T : Item
    {
        Task HandleAsync(T item);
    }
}
