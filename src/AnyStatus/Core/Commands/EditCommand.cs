using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Windows;

namespace AnyStatus
{
    public class EditCommand : ItemCommand
    {
        public EditCommand(Item target, Item source, Action close) : base(target)
        {
            Source = source;
            Close = close;
        }

        public Item Source { get; set; }

        public Action Close { get; set; }
    }

    public class EditCommandHandler : IHandler<EditCommand>
    {
        private readonly ISettingsStore _settingsStore;
        private readonly IJobScheduler _jobScheduler;

        public EditCommandHandler(ISettingsStore settingsStore, IJobScheduler jobScheduler)
        {
            _jobScheduler = Preconditions.CheckNotNull(jobScheduler, nameof(jobScheduler));
            _settingsStore = Preconditions.CheckNotNull(settingsStore, nameof(settingsStore));
        }

        public void Handle(EditCommand command)
        {
            if (command == null)
                throw new InvalidOperationException();

            if (!Validate(command.Item))
                return;

            command.Source.ReplaceWith(command.Item);

            command.Item.IsSelected = true;

            _jobScheduler.Remove(command.Source);

            _jobScheduler.Schedule(command.Item, includeChildren: false);

            _settingsStore.TrySave();

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