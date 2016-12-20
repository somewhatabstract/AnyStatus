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
        public AddCommand(Item item, Item parent, Action closeView) : base(item)
        {
            Parent = parent;
            CloseView = closeView;
        }

        public Item Parent { get; set; }

        public Action CloseView { get; set; }
    }

    public class AddCommandHandler : IHandler<AddCommand>
    {
        private readonly IJobScheduler _jobScheduler;
        private readonly ISettingsStore _settingsStore;
        private readonly IUsageReporter _usageReporter;
        private readonly IDialogService _dialogService;

        public AddCommandHandler(ISettingsStore settingsStore,
                                 IJobScheduler jobScheduler,
                                 IUsageReporter usageReporter,
                                 IDialogService dialogService)
        {
            _jobScheduler = Preconditions.CheckNotNull(jobScheduler, nameof(jobScheduler));
            _settingsStore = Preconditions.CheckNotNull(settingsStore, nameof(settingsStore));
            _usageReporter = Preconditions.CheckNotNull(usageReporter, nameof(usageReporter));
            _dialogService = Preconditions.CheckNotNull(dialogService, nameof(dialogService));
        }

        public void Handle(AddCommand command)
        {
            if (command == null)
                throw new InvalidOperationException();

            if (!Validate(command.Item))
                return;

            command.Parent.Add(command.Item);

            command.Item.IsSelected = true;

            _settingsStore.TrySave();

            _jobScheduler.Schedule(command.Item);

            _usageReporter.ReportEvent("Items", "Add", command.Item.GetType().Name);

            command.CloseView();
        }

        private bool Validate(Item item)
        {
            List<ValidationResult> results = null;

            item.Validate(out results);

            if (results.Any())
            {
                ShowValidationErrorsDialog(results);

                return false;
            }

            return true;
        }

        private void ShowValidationErrorsDialog(IEnumerable<ValidationResult> validationResults)
        {
            var message = new StringBuilder();

            foreach (var result in validationResults)
            {
                message.AppendLine(result.ErrorMessage);
            }

            _dialogService.ShowWarning(message.ToString(), "Validation");
        }
    }
}