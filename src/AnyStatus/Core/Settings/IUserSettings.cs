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

        void Save(bool reload = false);

        void RestoreDefault();

        void Import(string filePath);

        void Export(string filePath);
    }
}
