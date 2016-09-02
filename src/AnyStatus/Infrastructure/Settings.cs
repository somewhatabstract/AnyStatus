using AnyStatus.Models;
using System;
using System.Collections.ObjectModel;

namespace AnyStatus.Properties
{
    /// <summary>
    /// Extends Properties.Settings.Default
    /// </summary>
    public partial class Settings
    {
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public bool DebugMode
        {
            get
            {
                return (bool)this[nameof(DebugMode)];
            }
            set
            {
                this[nameof(DebugMode)] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public bool ReportAnonymousUsageData
        {
            get
            {
                return (bool)this[nameof(ReportAnonymousUsageData)];
            }
            set
            {
                this[nameof(ReportAnonymousUsageData)] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public Item RootItem
        {
            get
            {
                var rootItem = this[nameof(RootItem)] as Item;

                rootItem?.RestoreParentChildRelationship();

                return rootItem;
            }
            set
            {
                this[nameof(RootItem)] = value;
            }
        }

        //todo: remove Items property and restore-parents extension method

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public ObservableCollection<Item> Items
        {
            get
            {
                var items = ((ObservableCollection<Item>)(this[nameof(Items)]));

                return items?.RestoreParents();
            }
            set
            {
                this[nameof(Items)] = value;
            }
        }
    }
   
    public static class SettingsExtensions
    {
        /// <summary>
        /// Restore tree structure (parent-child relationships).
        /// </summary>
        [Obsolete]
        internal static ObservableCollection<Item> RestoreParents(this ObservableCollection<Item> items, Item parent = null)
        {
            foreach (var item in items)
            {
                item.Parent = parent;

                RestoreParents(item.Items, item);
            }

            return items;
        }
    }
}
