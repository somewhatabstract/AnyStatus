using FluentScheduler;
using System.Threading.Tasks;

namespace AnyStatus
{
    public interface IScheduledJob: IJob
    {
        Item Item { get; set; }

        Task ExecuteAsync();
    }
}
