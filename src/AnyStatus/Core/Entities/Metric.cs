using System.ComponentModel;

namespace AnyStatus
{
    public abstract class Metric : Item
    {
        private object _value;

        public Metric()
        {
            Interval = 1;
        }

        [Browsable(false)]
        public object Value
        {
            get { return _value; }
            set
            {
                _value = value;
                OnPropertyChanged();
            }
        }
    }
}
