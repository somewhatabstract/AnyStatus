using System;
using System.Threading.Tasks;

namespace AnyStatus
{
    public interface ISettingsStore
    {
        event EventHandler SettingsReset;

        AppSettings Settings { get; }

        bool TryInitialize();

        Task<bool> TryInitializeAsync();

        bool TrySave();

        bool TryRestoreDefaultSettings();

        bool TryImport(string filePath);

        bool TryExport(string filePath);
    }
}
