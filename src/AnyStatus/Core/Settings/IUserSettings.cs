using AnyStatus.Models;
using System;

namespace AnyStatus.Interfaces
{
    public interface IUserSettings
    {
        event EventHandler SettingsReset;

        Item RootItem { get; }
        bool DebugMode { get; set; }
        bool ReportAnonymousUsage { get; set; }
        string ClientId { get; set; }

        void Initialize();

        void Save(bool reload = false);

        void RestoreDefaultSettings();

        void Import(string filePath);

        void Export(string filePath);
    }
}
