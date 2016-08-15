using AnyStatus.Models;
using System.Collections.ObjectModel;

namespace AnyStatus.Interfaces
{
    //should replace Properties.Settings.Default
    public interface IUserSettings
    {
        ObservableCollection<Item> Items { get; set; }

        ObservableCollection<Server> Servers { get; set; }

        void Save();
        void Reset();
    }
}
