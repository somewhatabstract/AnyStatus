using AnyStatus.Interfaces;
using AnyStatus.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace AnyStatus
{
    /// <summary>
    /// Encapsulates access to Properties.Settings.Default
    /// </summary>
    public sealed class UserSettings : IUserSettings
    {
        public UserSettings()
        {
            try
            {
                Properties.Settings.Default.Items = Properties.Settings.Default.Items ?? new ObservableCollection<Item>();
                Properties.Settings.Default.Servers = Properties.Settings.Default.Servers ?? new ObservableCollection<Server>();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        public ObservableCollection<Item> Items
        {
            get
            {
                return Properties.Settings.Default.Items;
            }
            set
            {
                Properties.Settings.Default.Items = value;
            }
        }

        public ObservableCollection<Server> Servers
        {
            get
            {
                return Properties.Settings.Default.Servers;
            }
            set
            {
                Properties.Settings.Default.Servers = value;
            }
        }

        public void Save()
        {
            Properties.Settings.Default.Save();
        }

        public void Reset()
        {
            Properties.Settings.Default.Reset();

            Properties.Settings.Default.Items = new ObservableCollection<Item>();
            Properties.Settings.Default.Servers = new ObservableCollection<Server>();

            Save();

            Properties.Settings.Default.Reload();//does not refresh ui
        }
    }
}
