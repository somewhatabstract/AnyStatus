using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace AnyStatus
{
    public class NewViewModel : INotifyPropertyChanged
    {
        private Template _selectedTemplate;
        private readonly IUserSettings _userSettings;
        private readonly IJobScheduler _jobScheduler;
        private readonly IUsageReporter _usageReporter;
        private readonly ILogger _logger;
        private bool _canTest = true;

        public event EventHandler CloseRequested;

        public NewViewModel(
            IJobScheduler jobScheduler,
            IUserSettings userSettings,
            IUsageReporter usageReporter,
            IEnumerable<Template> templates,
            ILogger logger)
        {
            _jobScheduler = Preconditions.CheckNotNull(jobScheduler, nameof(jobScheduler));
            _userSettings = Preconditions.CheckNotNull(userSettings, nameof(userSettings));
            _usageReporter = Preconditions.CheckNotNull(usageReporter, nameof(usageReporter));
            _logger = Preconditions.CheckNotNull(logger, nameof(logger));
            Templates = Preconditions.CheckNotNull(templates, nameof(templates));

            SelectedTemplate = Templates?.FirstOrDefault();

            Initialize();
        }

        private void Initialize()
        {
            AddCommand = new RelayCommand(p => AddNewItem(p as Item));

            TestCommand = new RelayCommand(p => Test(p as Item), p => CanTest);

            CancelCommand = new RelayCommand(p => CloseRequested?.Invoke(this, EventArgs.Empty));
        }

        private void AddNewItem(Item item)
        {
            try
            {
                EnsureItemIsValid(item);

                if (Parent != null) Parent.Add(item);
                else _userSettings.RootItem.Add(item);

                _userSettings.Save();

                _jobScheduler.Schedule(item);

                _usageReporter.ReportEvent("Items", "Add", item.GetType().Name);

                CloseRequested?.Invoke(this, EventArgs.Empty);
            }
            catch (ValidationException)
            {
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to add new item");
            }
        }

        private void EnsureItemIsValid(Item item)
        {
            if (item == null)
                throw new InvalidOperationException("Item cannot be null.");

            List<ValidationResult> validationResults = null;

            item.Validate(out validationResults);

            if (validationResults.Any())
            {
                ShowValidationErrorsDialog(validationResults);

                throw new ValidationException();
            }
        }

        private async void Test(Item item)
        {
            try
            {
                CanTest = false;

                EnsureItemIsValid(item);

                var job = new ScheduledJob(_logger) { Item = item };

                await job.ExecuteAsync();

                //switch (item.State)
                //{
                //    case State.None:
                //        break;
                //    case State.Ok:
                //        MessageBox.Show("Ok", "Test", MessageBoxButton.OK, MessageBoxImage.Information);
                //        break;
                //    case State.Failed:
                //        MessageBox.Show("Test failed. See output window for more information.", "Test", MessageBoxButton.OK, MessageBoxImage.Warning);
                //        break;
                //    case State.Invalid:
                //        break;
                //    default:
                //        break;
                //}
            }
            catch (ValidationException)
            {
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Test failed");
            }
            finally
            {
                CanTest = true;
            }
        }

        private static void ShowValidationErrorsDialog(IEnumerable<ValidationResult> validationResults)
        {
            if (validationResults == null || !validationResults.Any())
                return;

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

        public bool CanTest
        {
            get { return _canTest; }
            set { _canTest = value; OnPropertyChanged(); }
        }

        #region Commands

        public ICommand AddCommand { get; set; }

        public ICommand TestCommand { get; set; }

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