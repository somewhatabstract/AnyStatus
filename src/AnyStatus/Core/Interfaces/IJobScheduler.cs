namespace AnyStatus
{
    public interface IJobScheduler
    {
        void Start();

        void Schedule(Item item, bool includeChildren = false);

        void Execute(Item item);

        void Remove(Item item);

        void RemoveAll();

        void ExecuteAll();

        void Stop();

        void Restart();

        void Disable(Item item);

        void Enable(Item item);

        bool Contains(Item item);
    }
}