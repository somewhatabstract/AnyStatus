namespace AnyStatus.Properties
{
    /// <summary>
    /// Extends Properties.Settings.Default
    /// </summary>
    public partial class Settings
    {
        //obsolete
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public Item RootItem
        {
            get
            {
                return this[nameof(RootItem)] as Item;
            }
            set
            {
                this[nameof(RootItem)] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public UserSettings UserSettings
        {
            get
            {
                return this[nameof(UserSettings)] as UserSettings;
            }
            set
            {
                this[nameof(UserSettings)] = value;
            }
        }
    }
}