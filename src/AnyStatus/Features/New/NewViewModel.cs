using AnyStatus.Interfaces;
using AnyStatus.Models;
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
        private IJobScheduler _jobScheduler;
        private ILogger _logger;

        public event EventHandler CloseRequested;

        public NewViewModel(IJobScheduler jobScheduler,
            IUserSettings userSettings,
            IEnumerable<Template> templates,
            ILogger logger)
        {
            if (jobScheduler == null)
                throw new ArgumentNullException(nameof(jobScheduler));
            if (userSettings == null)
                throw new ArgumentNullException(nameof(userSettings));
            if (templates == null)
                throw new ArgumentNullException(nameof(templates));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _logger = logger;
            _jobScheduler = jobScheduler;
            _userSettings = userSettings;

            Templates = templates;
            SelectedTemplate = Templates?.FirstOrDefault();

            Initialize();
        }

        private void Initialize()
        {
            AddCommand = new RelayCommand(p =>
            {
                try
                {
                    AddNewItem(p as Item);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Failed to add new item.");
                }
            });

            CancelCommand = new RelayCommand(p =>
            {
                CloseRequested?.Invoke(this, EventArgs.Empty);
            });
        }

        private void AddNewItem(Item item)
        {
            if (item == null)
                throw new InvalidOperationException("Item cannot be null.");

            var context = new ValidationContext(item, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();

            if (!Validator.TryValidateObject(item, context, results))
            {
                ShowValidationErrorsDialog(results);
                return;
            }

            if (item.Id == Guid.Empty)
                item.Id = Guid.NewGuid();

            if (Parent != null)
            {
                Parent.Add(item);
                Parent.IsExpanded = true;
            }
            else
            {
                _userSettings.RootItem.Add(item);
            }

            _userSettings.Save();

            _jobScheduler.Schedule(item);

            CloseRequested?.Invoke(this, EventArgs.Empty);
        }

        private static void ShowValidationErrorsDialog(IEnumerable<ValidationResult> validationResults)
        {
            var sb = new StringBuilder();

            foreach (var result in validationResults)
            {
                sb.AppendLine(result.ErrorMessage);
            }

            MessageBox.Show(sb.ToString(), "Validation", MessageBoxButton.OK, MessageBoxImage.Exclamation);
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

        public Item Parent { get; set; }

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