using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace AnyStatus
{
    public interface ISettingsStore : INotifyPropertyChanged
    {
        event EventHandler SettingsChanged;
        event EventHandler SettingsSourceChanged;

        UserSettings Settings { get; }

        bool TryInitialize();

        Task<bool> TryInitializeAsync();

        bool TrySave();

        void TryReload();

        bool TryRestoreDefaultSettings();

        bool TryImport(string filePath);

        bool TryExport(string filePath);
    }
}
