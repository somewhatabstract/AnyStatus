using System;
using System.Linq;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using System.Xml.Serialization;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus.Models
{
    [Serializable]
    [XmlInclude(typeof(Folder))]
    [XmlInclude(typeof(RootItem))]

    [XmlInclude(typeof(Ping))]
    [XmlInclude(typeof(TcpPort))]
    [XmlInclude(typeof(TfsBuild))]
    [XmlInclude(typeof(HttpStatus))]
    [XmlInclude(typeof(JenkinsBuild))]
    [XmlInclude(typeof(TeamCityBuild))]
    [XmlInclude(typeof(AppVeyorBuild))]
    [XmlInclude(typeof(TravisCIBuild))]
    [XmlInclude(typeof(WindowsService))]
    public class Item : INotifyPropertyChanged
    {
        #region Fields

        private bool _isExpanded;
        private bool _isEnabled;
        private bool _isEditing;
        private bool _isSelected;

        [NonSerialized]
        private Brush _brush;

        #endregion

        public Item()
        {
            IsEnabled = true;
            IsExpanded = true;
            Interval = 5;
            Brush = Brushes.Silver;
            Items = new ObservableCollection<Item>();
        }

        #region Properties

        [Browsable(false)]
        public Guid Id { get; set; }

        [Required]
        [PropertyOrder(0)]
        public string Name { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set { _isExpanded = value; OnPropertyChanged(); }
        }

        [Browsable(false)]
        [DisplayName("Enabled")]
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { _isEnabled = value; OnPropertyChanged(); }
        }

        public void Add(Item item)
        {
            if (Items == null)
                return;

            item.Parent = this;

            if (item is Folder)
            {
                var lastFolder = Items.LastOrDefault(k => k is Folder);

                if (lastFolder != null)
                {
                    var idx = Items.IndexOf(lastFolder);

                    if (idx + 1 < Items.Count())
                    {
                        Items.Insert(idx + 1, item);
                        return;
                    }
                }
            }

            Items.Add(item);
        }

        public void Remove()
        {
            if (Parent != null && Parent.Items != null)
            {
                Parent.Items.Remove(this);
            }
        }

        [Required]
        [Range(0, ushort.MaxValue, ErrorMessage = "Interval must be between 0 and 65535")]
        [Description("The interval in minutes")]
        public int Interval { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public Brush Brush
        {
            get { return _brush; }
            set { _brush = value; OnPropertyChanged(); }
        }

        [XmlIgnore]
        [Browsable(false)]
        public Item Parent { get; set; }

        [Browsable(false)]
        public ObservableCollection<Item> Items { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public bool IsEditing
        {
            get { return _isEditing; }
            set { _isEditing = value; OnPropertyChanged(); }
        }

        [XmlIgnore]
        [Browsable(false)]
        public bool IsSelected
        {
            get { return _isSelected; }
            set { _isSelected = value; OnPropertyChanged(); }
        }

        #endregion

        public void RestoreParentChildRelationship()
        {
            foreach (var child in Items)
            {
                child.Parent = this;

                child.RestoreParentChildRelationship();
            }
        }

        public void Reparent(Item target)
        {
            if (Parent == null || Parent.Items == null)
                return;

            if (target is Folder)
            {
                Remove();
                target.Add(this);
                //Parent.Items.Remove(this);
                //Parent = target;
                //Parent.Items.Insert(0, this);
                //Parent.Items.Add(this);
            }
            //else if (target.Parent == Parent)
            //{
            //    var targetIndex = Parent.Items.IndexOf(target);
            //    var sourceIndex = Parent.Items.IndexOf(this);
            //    if (sourceIndex < targetIndex)
            //    {
            //        target.Parent.Items.Insert(targetIndex + 1, this);
            //        Parent.Items.Remove(this);
            //    }
            //    else
            //    {
            //        Parent.Items.Remove(this);
            //        target.Parent.Items.Insert(targetIndex, this);
            //    }
            //}
        }

        public bool CanReparent(Item target)
        {
            return target != null &&
                   target is Folder &&
                   target != this.Parent &&
                   !this.IsParentOf(target);
            //&& (target is Folder || target.Parent == Parent);
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public IEnumerable GetErrors(string propertyName)
        {
            throw new NotImplementedException();
        }

        public bool IsParentOf(Item child)
        {
            if (this == child)
            {
                return true;
            }

            foreach (var item in Items)
            {

                if (item.IsParentOf(child))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion
    }
}