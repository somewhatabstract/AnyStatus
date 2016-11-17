using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace AnyStatus
{
    public class AddNewItemCommand : ICommand
    {
        private Item parent;
        private readonly Item _parent;
        private IJobScheduler _jobScheduler;
        private readonly ISettingsStore _settingsStore;
        private readonly IUsageReporter _usageReporter;

        public event EventHandler CanExecuteChanged;

        public AddNewItemCommand(ISettingsStore settingsStore,
                                 IJobScheduler jobScheduler,
                                 IUsageReporter usageReporter,
                                 Item parent)
        {
            _parent = parent;
            _settingsStore = Preconditions.CheckNotNull(settingsStore, nameof(settingsStore));
            _jobScheduler = Preconditions.CheckNotNull(jobScheduler, nameof(jobScheduler));
            _usageReporter = Preconditions.CheckNotNull(usageReporter, nameof(usageReporter));
        }

        public bool CanExecute(object parameter)
        {
            return parameter is Item;
        }

        public void Execute(object parameter)
        {
            var item = parameter as Item;

            if (item == null) return;

            if (_parent != null)
            {
                _parent.Add(item);
            }
            else
            {
                _settingsStore.Settings.RootItem.Add(item);
            }

            _settingsStore.TrySave();

            _jobScheduler.Schedule(item);

            _usageReporter.ReportEvent("Items", "Add", item.GetType().Name);

            //CloseRequested?.Invoke(this, EventArgs.Empty);
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
    }
}
