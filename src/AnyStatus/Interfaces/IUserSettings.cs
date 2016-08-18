using AnyStatus.Models;
using System.Collections.ObjectModel;

namespace AnyStatus.Interfaces
{
    public interface IUserSettings
    {
        ObservableCollection<Item> Items { get; }

        void Save();

        void Reset();
    }
}
