using System;
using System.Xml.Serialization;

namespace AnyStatus
{
    [Serializable]
    public class StateMetadata : NotifyPropertyChanged
    {
        private string _color;

        public StateMetadata()
        {
        }

        public StateMetadata(int value, int priority, string displayName, string color, string icon)
        {
            Value = value;
            DisplayName = displayName;
            Priority = priority;
            Color = color;
            Icon = icon;
        }

        public int Value { get; set; }

        public int Priority { get; set; }

        //[XmlIgnore]
        public string DisplayName { get; set; }

        public string Color
        {
            get { return _color; }
            set { _color = value; OnPropertyChanged(); }
        }

        //[XmlIgnore]
        public string Icon { get; set; }

        public StateMetadata Clone()
        {
            return (StateMetadata)MemberwiseClone();
        }
    }
}
