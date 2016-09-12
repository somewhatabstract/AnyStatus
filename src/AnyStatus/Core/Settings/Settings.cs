using AnyStatus.Models;

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
        public string ClientId
        {
            get
            {
                return (string)this[nameof(ClientId)];
            }
            set
            {
                this[nameof(ClientId)] = value;
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
    }
}