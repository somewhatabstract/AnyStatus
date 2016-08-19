﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using System.Xml.Serialization;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus.Models
{
    [Serializable]
    [XmlInclude(typeof(Folder))]
    [XmlInclude(typeof(JenkinsBuild))]
    [XmlInclude(typeof(HttpStatus))]
    [XmlInclude(typeof(Ping))]
    [XmlInclude(typeof(TcpPort))]
    public class Item : INotifyPropertyChanged
    {
        private string _name;
        private bool _isExpanded;
        private bool _isEnabled;
        private bool _isEditing;

        [NonSerialized]
        private Brush _brush;

        public Item()
        {
            Interval = 5;
            Brush = Brushes.Silver;
            Items = new ObservableCollection<Item>();
        }

        [Browsable(false)]
        public Guid Id { get; set; }

        [PropertyOrder(0)]
        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged(); }
        }

        [Browsable(false)]
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set { _isExpanded = value; OnPropertyChanged(); }
        }

        [Browsable(false)]
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { _isEnabled = value; OnPropertyChanged(); }
        }

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

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}