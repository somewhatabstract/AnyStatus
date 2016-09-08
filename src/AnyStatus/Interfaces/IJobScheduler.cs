using AnyStatus.Models;

namespace AnyStatus
{
    public interface IJobScheduler
    {
        void Schedule(Item item);

        void Reschedule(Item item);
    }
}