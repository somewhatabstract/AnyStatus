using AnyStatus.Interfaces;
using AnyStatus.Models;
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

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public ObservableCollection<Server> Servers
        {
            get
            {
                return ((ObservableCollection<Server>)(this[nameof(Servers)]));
            }
            set
            {
                this[nameof(Servers)] = value;
            }
        }
    }
   
    internal static class SettingsExtensions
    {
        /// <summary>
        /// Restore tree structure (parent-child relationships).
        /// </summary>
        public static ObservableCollection<Item> RestoreParents(this ObservableCollection<Item> items, Item parent = null)
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
