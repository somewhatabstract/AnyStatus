using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace AnyStatus.Models
{
    [Browsable(false)]
    public class Folder : Item
    {
        public Folder()
        {
            Items.CollectionChanged += Items_CollectionChanged;
        }

        [Browsable(false)]
        public new int Interval { get; set; }

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Unsubscribe(e);

            Subscribe(e);
        }

        private void Subscribe(NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (INotifyPropertyChanged item in e.NewItems)
                {
                    item.PropertyChanged += Item_PropertyChanged;
                }
            }
        }

        private void Unsubscribe(NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (INotifyPropertyChanged item in e.OldItems)
                {
                    item.PropertyChanged -= Item_PropertyChanged;
                }
            }
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(State)) && Items.Any())
            {
                State = Items.Select(item => item.State).Aggregate((a, b) => a > b ? a : b);
            }
        }
    }
}
