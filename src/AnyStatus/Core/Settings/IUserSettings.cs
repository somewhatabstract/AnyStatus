using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace AnyStatus
{
    public interface IUserSettings : INotifyPropertyChanged
    {
        event EventHandler SettingsReset;

        string ClientId { get; set; }

        Item RootItem { get; }

        bool DebugMode { get; set; }

        bool ReportAnonymousUsage { get; set; }

        bool ShowStatusIcons { get; set; }

        bool ShowStatusColors { get; set; }

        Theme Theme { get; set; }

        Task InitializeAsync();

        void Initialize();

        void Save(bool reload = false);

        void RestoreDefaultSettings();

        void Import(string filePath);

        void Export(string filePath);
    }
}
