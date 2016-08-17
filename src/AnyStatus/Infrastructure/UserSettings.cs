using AnyStatus.Interfaces;
using AnyStatus.Models;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace AnyStatus
{
    /// <summary>
    /// Encapsulates access to Properties.Settings.Default
    /// </summary>
    public class UserSettings : IUserSettings
    {
        public UserSettings()
        {
            try
            {
                if (Properties.Settings.Default.Items == null)
                {
                    Properties.Settings.Default.Items = new ObservableCollection<Item>();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
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

        public void Save()
        {
            try
            {
                Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public void Reset()
        {
            try
            {
                Properties.Settings.Default.Reset();

                Properties.Settings.Default.Items = new ObservableCollection<Item>();

                Properties.Settings.Default.Save();

                Properties.Settings.Default.Reload();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}
