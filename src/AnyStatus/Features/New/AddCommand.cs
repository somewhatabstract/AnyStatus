using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Windows;

namespace AnyStatus
{
    public class AddCommand : ItemCommand
    {
        public AddCommand(Item item, Item parent, Action close) : base(item)
        {
            Parent = parent;
            Close = close;
        }

        public Item Parent { get; set; }

        public Action Close { get; set; }
    }

    public class AddCommandHandler : IHandler<AddCommand>
    {
        private readonly ISettingsStore _settingsStore;
        private readonly IJobScheduler _jobScheduler;
        private readonly IUsageReporter _usageReporter;

        public AddCommandHandler(ISettingsStore settingsStore, 
                                 IJobScheduler jobScheduler, 
                                 IUsageReporter usageReporter)
        {
            _jobScheduler = Preconditions.CheckNotNull(jobScheduler, nameof(jobScheduler));
            _settingsStore = Preconditions.CheckNotNull(settingsStore, nameof(settingsStore));
            _usageReporter = Preconditions.CheckNotNull(usageReporter, nameof(usageReporter));
        }

        public void Handle(AddCommand command)
        {
            if (command?.Item == null)
                return;

            if (!Validate(command.Item))
            {
                return;
            }

            command.Parent.Add(command.Item);

            _settingsStore.TrySave();

            _jobScheduler.Schedule(command.Item);

            _usageReporter.ReportEvent("Items", "Add", command.Item.GetType().Name);

            command.Close();
        }

        private bool Validate(Item item)
        {
            List<ValidationResult> validationResults = null;

            item.Validate(out validationResults);

            if (validationResults.Any())
            {
                ShowValidationErrorsDialog(validationResults);

                return false;
            }

            return true;
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

            System.Windows.MessageBox.Show(sb.ToString(), "Validation", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }
    }
}