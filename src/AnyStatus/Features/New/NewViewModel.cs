using AnyStatus.Infrastructure;
using AnyStatus.Interfaces;
using AnyStatus.Models;
using FluentScheduler;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace AnyStatus.ViewModels
{
    public class NewViewModel : INotifyPropertyChanged
    {
        private Template _selectedTemplate;
        private IUserSettings _userSettings;

        public event EventHandler CloseRequested;

        public NewViewModel(IUserSettings userSettings, IEnumerable<Template> templates)
        {
            if (userSettings == null)
                throw new ArgumentNullException(nameof(userSettings));

            if (templates == null)
                throw new ArgumentNullException(nameof(templates));

            _userSettings = userSettings;

            Templates = templates;

            SelectedTemplate = Templates?.FirstOrDefault();

            Initialize();
        }

        private void Initialize()
        {
            AddCommand = new RelayCommand(p =>
            {
                var item = p as Item;

                if (item == null)
                {
                    return;
                }

                var context = new ValidationContext(item, serviceProvider: null, items: null);
                var results = new List<ValidationResult>();
                var isValid = Validator.TryValidateObject(item, context, results);

                if (!isValid)
                {
                    var sb = new StringBuilder();

                    foreach (var validationResult in results)
                    {
                        sb.AppendLine(validationResult.ErrorMessage);
                    }

                    MessageBox.Show(sb.ToString(), "Validation", MessageBoxButton.OK, MessageBoxImage.Exclamation);

                    return;
                }

                item.Id = Guid.NewGuid();

                if (Parent != null)
                {
                    item.Parent = Parent;
                    Parent.Items.Add(item);
                    Parent.IsExpanded = true;
                }
                else
                {
                    _userSettings.RootItem.Items.Add(item);
                }

                _userSettings.Save();

                JobManager.AddJob(new ScheduledJob(item),
                    schedule => schedule.WithName(item.Id.ToString()).ToRunNow().AndEvery(item.Interval).Minutes());

                CloseRequested?.Invoke(this, EventArgs.Empty);
            });

            CancelCommand = new RelayCommand(p =>
            {
                CloseRequested?.Invoke(this, EventArgs.Empty);
            });
        }

        public Template SelectedTemplate
        {
            get
            {
                return _selectedTemplate;
            }
            set
            {
                _selectedTemplate = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<Template> Templates { get; set; }

        public Item Parent { get; internal set; }

        #region Commands

        public ICommand AddCommand { get; set; }

        public ICommand CancelCommand { get; set; }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}