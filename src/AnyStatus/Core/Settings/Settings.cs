﻿using System.Xml.Serialization;

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
                return this[nameof(RootItem)] as Item;
            }
            set
            {
                this[nameof(RootItem)] = value;
            }
        }

        [XmlElement(ElementName = "Settings")]//todo: fix name
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