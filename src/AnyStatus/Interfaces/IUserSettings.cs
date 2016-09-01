using AnyStatus.Models;

namespace AnyStatus.Interfaces
{
    public interface IUserSettings
    {
        Item RootItem { get; }

        void Save();

        void Reset();
    }
}
