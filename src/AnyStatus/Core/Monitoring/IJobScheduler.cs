namespace AnyStatus
{
    public interface IJobScheduler
    {
        void Start();

        void Schedule(Item item, bool includeChildren = false);

        void Reschedule(Item item);

        void Execute(Item item);

        void Remove(Item item);

        void RemoveAll();

        void ExecuteAll();

        void Stop();

        void Restart();
    }
}