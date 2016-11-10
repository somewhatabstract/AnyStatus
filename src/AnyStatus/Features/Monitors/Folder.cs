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
            if (e.NewItems == null) return;

            foreach (INotifyPropertyChanged item in e.NewItems)
            {
                item.PropertyChanged += Item_PropertyChanged;
            }

            CalculateState();
        }

        private void Unsubscribe(NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems == null) return;

            foreach (INotifyPropertyChanged item in e.OldItems)
            {
                item.PropertyChanged -= Item_PropertyChanged;
            }

            CalculateState();
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(State)))
            {
                CalculateState();
            }
        }

        public void CalculateState()
        {
            if (Items != null && Items.Any())
            {
                State = Items.Aggregate((a, b) => a.State > b.State ? a : b).State;
            }
        }
    }
}
