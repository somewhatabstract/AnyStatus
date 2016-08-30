using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Media;
using System.Xml.Serialization;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus.Models
{
    [Serializable]
    [XmlInclude(typeof(Ping))]
    [XmlInclude(typeof(Folder))]
    [XmlInclude(typeof(TcpPort))]
    [XmlInclude(typeof(TfsBuild))]
    [XmlInclude(typeof(HttpStatus))]
    [XmlInclude(typeof(JenkinsBuild))]
    [XmlInclude(typeof(TeamCityBuild))]
    [XmlInclude(typeof(AppVeyorBuild))]
    [XmlInclude(typeof(TravisCIBuild))]
    [XmlInclude(typeof(WindowsService))]
    public class Item : INotifyPropertyChanged, IDataErrorInfo
    {
        #region Fields

        private bool _isExpanded;
        private bool _isEnabled;
        private bool _isEditing;

        [NonSerialized]
        private Brush _brush;

        #endregion

        public Item()
        {
            IsEnabled = true;
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

        [Range(0, ushort.MaxValue)]
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
        public string Error
        {
            get
            {
                return string.Empty;
            }
        }

        [XmlIgnore]
        [Browsable(false)]
        public string this[string columnName]
        {
            get
            {
                var value = GetType().GetProperty(columnName).GetValue(this, null);
                var context = new ValidationContext(this, null, null) { MemberName = columnName };
                var validationResults = new List<ValidationResult>();

                if (Validator.TryValidateProperty(value, context, validationResults))
                {
                    return null;
                }

                var sb = new StringBuilder();

                foreach (var validationResult in validationResults)
                {
                    sb.AppendLine(validationResult.ErrorMessage);
                }

                return sb.ToString();
            }
        }

        #endregion

        #region INotifyDataErrorInfo

        //private Dictionary<string, List<string>> _fieldNameToErrors = new Dictionary<string, List<string>>();

        //public bool HasErrors
        //{
        //    get
        //    {
        //        return (_fieldNameToErrors.Count > 0);
        //    }
        //}

        //public string Error
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //public string this[string columnName]
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //protected void AddError(string propertyName, string error, bool isWarning)
        //{
        //    List<string> errors = null;

        //    if (!_fieldNameToErrors.TryGetValue(propertyName, out errors))
        //    {
        //        errors = new List<string>();
        //        _fieldNameToErrors.Add(propertyName, errors);
        //    }

        //    if (!errors.Contains(error))
        //    {
        //        if (isWarning)
        //        {
        //            errors.Add(error);
        //        }
        //        else
        //        {
        //            errors.Insert(0, error);
        //        }

        //        RaiseErrorsChanged(propertyName);
        //    }
        //}

        //protected void RemoveError(string propertyName, string error)
        //{
        //    List<string> errors = null;

        //    if (_fieldNameToErrors.TryGetValue(propertyName, out errors))
        //    {
        //        errors.Remove(error);

        //        if (errors.Count == 0)
        //        {
        //            _fieldNameToErrors.Remove(propertyName);
        //            RaiseErrorsChanged(propertyName);
        //        }
        //    }
        //}

        //public void RaiseErrorsChanged([CallerMemberName] string propertyName = null)
        //{
        //    ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        //}

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
            //if (string.IsNullOrWhiteSpace(propertyName))
            //{
            //    return _fieldNameToErrors.Values;
            //}

            //List<string> errors = null;

            //_fieldNameToErrors.TryGetValue(propertyName, out errors);

            //return errors;
        }

        #endregion
    }
}