using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace AnyStatus
{
    [Browsable(false)]
    public class Folder : Item
    {
        public Folder() : this(aggregateState: true)
        {
        }

        public Folder(bool aggregateState)
        {
            if (aggregateState)
                Items.CollectionChanged += Items_CollectionChanged;
        }

        [Browsable(false)]
        public new int Interval { get; set; }

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Unsubscribe(e.OldItems);

            Subscribe(e.NewItems);

            AggregateState();
        }

        private void Subscribe(IList items)
        {
            if (items == null)
                return;

            foreach (INotifyPropertyChanged item in items)
                item.PropertyChanged += Item_PropertyChanged;
        }

        private void Unsubscribe(IList items)
        {
            if (items == null)
                return;

            foreach (INotifyPropertyChanged item in items)
                item.PropertyChanged -= Item_PropertyChanged;
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(State)))
                AggregateState();
        }

        public void AggregateState()
        {
            State = Items != null && Items.Any() ?
                        Items.Aggregate(ByPriority).State :
                            State.None;
        }

        private static Item ByPriority(Item a, Item b)
        {
            return a.State.Metadata.Priority > b.State.Metadata.Priority ? a : b;
        }

        #region ICloneable

        public override object Clone()
        {
            var clone = (Item)base.Clone();

            foreach (var item in Items)
                if (item != null)
                    clone.Add((Item)item.Clone());

            return clone;
        }

        #endregion
    }
}
