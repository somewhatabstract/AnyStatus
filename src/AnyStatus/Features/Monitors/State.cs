using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AnyStatus
{
    public class State : Enumeration<State, int>, INotifyPropertyChanged
    {
        private StateMetadata _stateMetadata;

        public static readonly State None = new State(0, 0, "None", "Silver", "Blank.png");
        public static readonly State Unknown = new State(1, 1, "Unknown", "Silver", "StatusHelp_gray_16x.png");
        public static readonly State Disabled = new State(2, 2, "Disabled", "Silver", "StatusPause_grey_16x.png");
        public static readonly State Canceled = new State(3, 3, "Canceled", "Gray", "StatusStop_grey_16x.png");
        public static readonly State Ok = new State(4, 4, "Ok", "Green", "StatusOK_grey_16x.png");
        public static readonly State Open = new State(5, 5, "Open", "Green", "StatusInformation_grey_16x.png");
        public static readonly State Closed = new State(6, 6, "Closed", "Red", "StatusOK_grey_16x.png");
        public static readonly State PartiallySucceeded = new State(7, 7, "Partially Succeeded", "Orange", "StatusInvalid_grey_16x");
        public static readonly State Failed = new State(8, 8, "Failed", "Red", "StatusCriticalError_grey_16x.png");
        public static readonly State Invalid = new State(9, 9, "Invalid", "DarkRed", "StatusWarning_grey_16x.png");
        public static readonly State Error = new State(10, 10, "Error", "DarkRed", "StatusWarning_grey_16x.png");
        public static readonly State Running = new State(11, 11, "Running", "DodgerBlue", "StatusRun_grey_16x.png");

        private State(int value, int priority, string displayName, string color, string icon) :
            base(value)
        {
            Metadata = new StateMetadata(value, priority, displayName, color, icon);
        }

        public StateMetadata Metadata
        {
            get { return _stateMetadata; }
            private set { _stateMetadata = value; OnPropertyChanged(); }
        }

        public static void SetMetadata(StateMetadata[] metadataArray)
        {
            if (metadataArray == null)
                return;

            var states = GetAll().ToDictionary(k => k.Value, v => v);

            foreach (var metadata in metadataArray)
            {
                if (states.ContainsKey(metadata.Value))
                {
                    states[metadata.Value].Metadata = metadata;
                }
            }
        }

        public static StateMetadata[] GetMetadata()
        {
            return GetAll().Select(state => state.Metadata).ToArray();
        }

        #region IXmlSerializable

        //public void ReadXml(XmlReader reader)
        //{
        //    reader.MoveToContent();
        //    reader.ReadStartElement();

        //    var value = reader.ReadElementContentAsInt();

        //    var state = FromValue(value);

        //    Priority = state.Priority = reader.ReadElementContentAsInt();
        //    Color = state.Color = reader.ReadElementContentAsString();
        //    Icon = state.Icon = reader.ReadElementContentAsString();
        //}

        //public void WriteXml(XmlWriter writer)
        //{
        //    writer.WriteElementString("Value", Value.ToString());
        //    writer.WriteElementString("Priority", Priority.ToString());
        //    writer.WriteElementString("Color", Color);
        //    writer.WriteElementString("Icon", Icon);
        //}

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler SettingsReset;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
