using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AnyStatus
{
    public class TestCommand : ItemCommand
    {
        public TestCommand(Item item, Action<bool> canExecute, Action<string> message) : base(item)
        {
            CanExecute = canExecute;
            Message = message;
        }

        public Action<bool> CanExecute { get; set; }

        public Action<string> Message { get; set; }
    }

    public class TestCommandHandler : IHandler<TestCommand>
    {
        private readonly Func<IScheduledJob> _jobFactory;

        public TestCommandHandler(Func<IScheduledJob> jobFactory)
        {
            _jobFactory = Preconditions.CheckNotNull(jobFactory, nameof(jobFactory));
        }

        public void Handle(TestCommand command)
        {
            if (command == null)
                throw new InvalidOperationException();

            if (!Validate(command.Item))
                return;

            command?.CanExecute(false);

            command?.Message("Testing...");

            var job = _jobFactory();

            job.Item = command.Item;

            Task.Run(job.ExecuteAsync)
                .ContinueWith(task =>
                {
                    command?.CanExecute(true);
                    command?.Message(job.Item.State.Metadata.DisplayName);
                })
                .ConfigureAwait(false);
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