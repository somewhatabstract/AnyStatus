﻿using AnyStatus.Models;
using System;
using System.ComponentModel;

namespace AnyStatus.Interfaces
{
    public interface IUserSettings : INotifyPropertyChanged
    {
        event EventHandler SettingsReset;

        string ClientId { get; set; }

        Item RootItem { get; }

        bool DebugMode { get; set; }

        bool ReportAnonymousUsage { get; set; }

        void Initialize();

        void Save(bool reload = false);

        void RestoreDefaultSettings();

        void Import(string filePath);

        void Export(string filePath);
    }
}
