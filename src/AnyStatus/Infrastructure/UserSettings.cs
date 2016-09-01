using AnyStatus.Infrastructure;
using AnyStatus.Interfaces;
using AnyStatus.Models;
using System;
using System.Diagnostics;

//todo: write debug log

namespace AnyStatus
{
    /// <summary>
    /// Encapsulates access to Properties.Settings.Default
    /// </summary>
    public class UserSettings : IUserSettings
    {
        private Item _rootItem;

        public UserSettings()
        {
            try
            {
                if (Properties.Settings.Default.RootItem == null)
                {
                    Reset();
                }
                else
                {
                    _rootItem = Properties.Settings.Default.RootItem;
                }

                Upgrade();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private void Upgrade()
        {
            if (Properties.Settings.Default.Items != null)
            {
                _rootItem.Items = Properties.Settings.Default.Items;
                _rootItem.RestoreParentChildReferences();
                Properties.Settings.Default.Items = null;
                Save();
            }
        }

        public Item RootItem
        {
            get
            {
                return _rootItem;
            }
        }

        public void Save()
        {
            try
            {
                Retry.Do(() =>
                {
                    Properties.Settings.Default.RootItem = _rootItem;
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

                Properties.Settings.Default.RootItem = new Item();

                Properties.Settings.Default.Items = new System.Collections.ObjectModel.ObservableCollection<Item>();//obsolete

                Properties.Settings.Default.Save();

                Properties.Settings.Default.Reload();

                _rootItem = Properties.Settings.Default.RootItem;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}
