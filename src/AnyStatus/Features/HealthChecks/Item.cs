using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using System.Xml.Serialization;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus.Models
{
    [Serializable]
    //Folders////////////////////
    [XmlInclude(typeof(Folder))]
    [XmlInclude(typeof(RootItem))]
    //Plug-ins////////////////////
    [XmlInclude(typeof(Ping))]
    [XmlInclude(typeof(TcpPort))]
    [XmlInclude(typeof(TfsBuild))]
    [XmlInclude(typeof(HttpStatus))]
    [XmlInclude(typeof(GitHubIssue))]
    [XmlInclude(typeof(JenkinsBuild))]
    [XmlInclude(typeof(TeamCityBuild))]
    [XmlInclude(typeof(AppVeyorBuild))]
    [XmlInclude(typeof(TravisCIBuild))]
    [XmlInclude(typeof(WindowsService))]
    public class Item : INotifyPropertyChanged, ICloneable
    {
        #region Fields

        private string _name;
        private Item _parent;
        private int _interval;
        private ObservableCollection<Item> _items;

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
        [Category("General")]
        [PropertyOrder(0)]
        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged(); }
        }

        [Browsable(false)]
        public ObservableCollection<Item> Items
        {
            get { return _items; }
            set { _items = value; OnPropertyChanged(); }
        }

        [XmlIgnore]
        [Browsable(false)]
        public Item Parent
        {
            get { return _parent; }
            set { _parent = value; OnPropertyChanged(); }
        }

        [Required]
        [Category("General")]
        [Range(0, ushort.MaxValue, ErrorMessage = "Interval must be between 0 and 65535")]
        [Description("The interval in minutes")]
        public int Interval
        {
            get { return _interval; }
            set { _interval = value; OnPropertyChanged(); }
        }

        [XmlIgnore]
        [Browsable(false)]
        public Brush Brush
        {
            get { return _brush; }
            set { _brush = value; OnPropertyChanged(); }
        }

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

        #region Methods

        public void Add(Item item)
        {
            if (item == null || Items == null)
                throw new InvalidOperationException();

            item.Parent = this;

            Items.Add(item);
        }

        public void Collapse()
        {
            IsExpanded = false;
        }

        public void CollapseAll()
        {
            Collapse();

            foreach (var item in Items)
            {
                item.CollapseAll();
            }
        }

        public void Expand()
        {
            IsExpanded = true;
        }

        public void ExpandAll()
        {
            Expand();

            foreach (var item in Items)
            {
                item.ExpandAll();
            }
        }

        public void Delete()
        {
            if (Parent == null || Parent.Items == null)
                throw new InvalidOperationException();

            if (Parent.Items.Remove(this))
            {
                Parent = null;
            }
        }

        public void RestoreParentChildRelationship()
        {
            if (Items == null)
                throw new InvalidOperationException();

            foreach (var item in Items)
            {
                item.Parent = this;

                item.RestoreParentChildRelationship();
            }
        }

        public void MoveUp()
        {
            if (CanMoveUp())
            {
                var index = Parent.Items.IndexOf(this);

                Parent.Items.Move(index, index - 1);
            }
        }

        public bool CanMoveUp()
        {
            if (Parent == null || Parent.Items == null)
                throw new InvalidOperationException();

            return Parent.Items.IndexOf(this) > 0;
        }

        public void MoveDown()
        {
            if (CanMoveDown())
            {
                var index = Parent.Items.IndexOf(this);

                Parent.Items.Move(index, index + 1);
            }
        }

        public bool CanMoveDown()
        {
            if (Parent == null || Parent.Items == null)
                throw new InvalidOperationException();

            return Parent.Items.IndexOf(this) + 1 < Parent.Items.Count();
        }

        /// <summary>
        /// Move item into folder.
        /// </summary>
        /// <param name="folder">The target folder.</param>
        private void MoveTo(Folder folder)
        {
            if (folder.Items.Contains(this))
                throw new InvalidOperationException("Target folder already contains this item.");

            Delete();

            folder.Add(this);
        }

        public bool CanMoveTo(Item target)
        {
            return target != null &&
                   target != this.Parent &&
                  (target is Folder || target.Parent == this.Parent) &&
                   this.IsNotParentOf(target);
        }

        public void MoveTo(Item target)
        {
            if (target is Folder)
            {
                MoveTo(target as Folder);
            }
            else if (this.Parent == target.Parent)
            {
                MoveToPositionOf(target);
            }
        }

        /// <summary>
        /// Change item position in folder.
        /// </summary>
        /// <param name="target"></param>
        private void MoveToPositionOf(Item target)
        {
            if (this.Parent != target.Parent)
                throw new InvalidOperationException("Item can only be moved within a folder.");

            var targetIndex = Parent.Items.IndexOf(target);
            var sourceIndex = Parent.Items.IndexOf(this);

            if (sourceIndex < targetIndex)
            {
                Parent.Items.Insert(targetIndex + 1, this);
                Parent.Items.Remove(this);
            }
            else
            {
                Parent.Items.Remove(this);
                Parent.Items.Insert(targetIndex, this);
            }
        }

        public void ReplaceWith(Item item)
        {
            var index = Parent.Items.IndexOf(this);

            Parent.Items[index] = item;
        }

        public bool ShouldSchedule()
        {
            return this is IScheduledItem &&
                   this.IsEnabled &&
                   this.Id != Guid.Empty;
        }

        public bool HasChildren()
        {
            return Items != null && Items.Any();
        }

        #endregion

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

        public bool IsNotParentOf(Item item)
        {
            return !IsParentOf(item);
        }



        #endregion

        #region ICloneable

        public object Clone()
        {
            return MemberwiseClone();
        }

        #endregion
    }
}