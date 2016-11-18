using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AnyStatus
{
    public class State : Enumeration<State, int>, INotifyPropertyChanged
    {
        private StateMetadata _stateMetadata;

        public static readonly State None = new State(0);
        public static readonly State Unknown = new State(1);
        public static readonly State Disabled = new State(2);
        public static readonly State Canceled = new State(3);
        public static readonly State Ok = new State(4);
        public static readonly State Open = new State(5);
        public static readonly State Closed = new State(6);
        public static readonly State PartiallySucceeded = new State(7);
        public static readonly State Failed = new State(8);
        public static readonly State Invalid = new State(9);
        public static readonly State Error = new State(10);
        public static readonly State Running = new State(11);

        private State(int value) : base(value)
        {
        }
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
                    states[metadata.Value].Metadata.Color = metadata.Color;
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

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
