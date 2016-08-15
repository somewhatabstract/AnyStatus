using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace AnyStatus.Models
{
    [Serializable]
    [XmlInclude(typeof(Job))]
    [XmlInclude(typeof(Folder))]
    public class Item : INotifyPropertyChanged
    {
        private bool _isExpanded;

        public Item()
        {
            Items = new ObservableCollection<Item>();
        }

        public Item(Item parent) : this()
        {
            Parent = parent;
        }

        public string Name { get; set; }

        [Browsable(false)]
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set {
                _isExpanded = value;
                OnPropertyChanged();
            }
        }

        [Browsable(false)]
        public bool IsSelected { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public Item Parent { get; set; }

        [Browsable(false)]
        public ObservableCollection<Item> Items { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
