using System.Xml.Serialization;

namespace AnyStatus.Properties
{
    /// <summary>
    /// Extends Properties.Settings.Default
    /// </summary>
    public partial class Settings
    {

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

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public Theme Theme
        {
            get
            {
                return this[nameof(Theme)] as Theme;
            }
            set
            {
                this[nameof(Theme)] = value;
            }
        }

        [XmlElement(ElementName = "Settings")]
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public AppSettings AppSettings
        {
            get
            {
                return this[nameof(AppSettings)] as AppSettings;
            }
            set
            {
                this[nameof(AppSettings)] = value;
            }
        }
    }
}