using AnyStatus.Infrastructure;
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
        private ObservableCollection<Item> _items;

        public UserSettings()
        {
            try
            {
                //Properties.Settings.Default.Upgrade()

                _items = Properties.Settings.Default.Items ?? new ObservableCollection<Item>();
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
                return _items;
            }
        }

        public void Save()
        {
            try
            {
                Retry.Do(() =>
                {
                    Properties.Settings.Default.Items = _items;
                    Properties.Settings.Default.Save();
                },
                TimeSpan.FromSeconds(1), retryCount: 3);
            }
            catch (AggregateException ex)
            {
                Debug.WriteLine(ex.Flatten());
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
