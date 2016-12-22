using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus
{
    [Serializable]
    [CategoryOrder("General", 1)]
    [XmlInclude(typeof(Folder))]
    [XmlInclude(typeof(RootItem))]
    [XmlInclude(typeof(Ping))]
    [XmlInclude(typeof(TcpPort))]
    [XmlInclude(typeof(TfsBuild))]
    [XmlInclude(typeof(BatchFile))]
    [XmlInclude(typeof(PowerShellScript))]
    [XmlInclude(typeof(HttpStatus))]
    [XmlInclude(typeof(GitHubIssue))]
    [XmlInclude(typeof(JenkinsBuild))]
    [XmlInclude(typeof(TeamCityBuild))]
    [XmlInclude(typeof(AppVeyorBuild))]
    [XmlInclude(typeof(TravisCIBuild))]
    [XmlInclude(typeof(WindowsService))]
    public class Item : INotifyPropertyChanged, ICloneable, IValidatable
    {
        #region Fields

        private string _name;
        private Item _parent;
        private int _interval;
        private bool _isExpanded;
        private bool _isEnabled;
        private bool _isEditing;
        private bool _isSelected;
        private ObservableCollection<Item> _items;

        [NonSerialized]
        private State _state;

        #endregion

        public Item()
        {
            IsEnabled = true;
            IsExpanded = true;
            Interval = 5;
            State = State.None;
            Items = new ObservableCollection<Item>(); //todo: set for root and folder items only.
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
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set { _isExpanded = value; OnPropertyChanged(); }
        }

        [XmlIgnore]
        [Browsable(false)]
        public State State
        {
            get { return _state; }
            set { _state = value; OnPropertyChanged(); }
        }

        [Browsable(false)]
        [DisplayName("Enabled")]
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;

                OnPropertyChanged();

                if (_isEnabled == false)
                    State = State.Disabled; //todo: this is an issue since Item does not control its own state.
            }
        }

        [Browsable(false)]
        public bool IsDisabled
        {
            get { return !_isEnabled; }
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

        public virtual void Add(Item item)
        {
            if (item == null || Items == null)
                throw new InvalidOperationException();

            if (item.Id == Guid.Empty)
                item.Id = Guid.NewGuid();

            item.Parent = this;

            Items.Add(item);

            IsExpanded = true;
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

        public virtual void Delete()
        {
            if (Parent == null || Parent.Items == null)
                throw new InvalidOperationException();

            if (Parent.Items.Remove(this))
            {
                Parent = null;
            }
        }

        public virtual Item Duplicate()
        {
            if (Parent == null)
                throw new InvalidOperationException("Item must have a Parent");

            var item = (Item)Clone();

            item.Id = Guid.NewGuid();

            item.Name = GetNextName();

            Parent.Add(item);

            item.MoveAfter(this);

            return item;
        }

        private string GetNextName()
        {
            if (Parent == null)
                throw new InvalidOperationException("Item must have a Parent");

            int i = 1;
            var name = string.Empty;

            do name = $"{Name} #{i++}";
            while (Parent.Items.Any(item => item.Name == name));

            return name;
        }

        public void RestoreParentChildRelationship()
        {
            if (Items == null)
                return;

            foreach (var item in Items)
            {
                item.Parent = this;

                item.RestoreParentChildRelationship();
            }
        }

        public virtual void MoveUp()
        {
            if (CanMoveUp())
            {
                var index = Parent.Items.IndexOf(this);

                Parent.Items.Move(index, index - 1);
            }
        }

        public bool CanMoveUp()
        {
            return Parent != null &&
                   Parent.Items != null &&
                   Parent.Items.IndexOf(this) > 0;
        }

        public virtual void MoveDown()
        {
            if (CanMoveDown())
            {
                var index = Parent.Items.IndexOf(this);

                Parent.Items.Move(index, index + 1);
            }
        }

        public bool CanMoveDown()
        {
            return Parent != null &&
                   Parent.Items != null &&
                   Parent.Items.IndexOf(this) + 1 < Parent.Items.Count();
        }

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
                   !IsAncestorOf(target);
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

        private void MoveAfter(Item target)
        {
            var targetIndex = Parent.Items.IndexOf(target);
            Parent.Items.Remove(this);
            Parent.Items.Insert(targetIndex + 1, this);
        }

        public virtual void ReplaceWith(Item item)
        {
            if (Parent == null)
                throw new InvalidOperationException();

            item.Parent = Parent;

            var index = Parent.Items.IndexOf(this);

            Parent.Items[index] = item;
        }

        public bool IsSchedulable()
        {
            return this.IsEnabled &&
                   this is IScheduledItem &&
                   this.Id != Guid.Empty;
        }

        public bool ContainsElements()
        {
            return Items != null && Items.Any();
        }

        public bool ContainsElements(Type type)
        {
            if (!ContainsElements()) return false;

            foreach (var item in Items)
            {
                if (item.GetType().Equals(type)) return true;

                else if (item.ContainsElements(type)) return true;
            }

            return false;
        }

        public bool Contains(Item item)
        {
            return Items != null && Items.Contains(item);
        }

        #region Validation

        public bool IsValid()
        {
            var context = new ValidationContext(this, serviceProvider: null, items: null);

            return Validator.TryValidateObject(this, context, null);
        }

        public bool Validate(out List<ValidationResult> results)
        {
            results = new List<ValidationResult>();

            var context = new ValidationContext(this, serviceProvider: null, items: null);

            return Validator.TryValidateObject(this, context, results);
        } 

        #endregion

        public bool IsAncestorOf(Item child)
        {
            if (this == child)
            {
                return true;
            }

            foreach (var item in Items)
            {

                if (item.IsAncestorOf(child))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public IEnumerable GetErrors(string propertyName)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ICloneable

        public virtual object Clone()
        {
            var clone = (Item)Activator.CreateInstance(GetType());

            clone.Id = Guid.NewGuid();

            var properties = from p in GetType().GetProperties()
                             where p.CanWrite &&
                                   p.Name != nameof(Id) &&
                                   p.Name != nameof(Parent) &&
                                   p.Name != nameof(Items)
                             select p;

            foreach (var property in properties)
                property.SetValue(clone, property.GetValue(this, null), null);

            return clone;
        }

        

        #endregion
    }
}