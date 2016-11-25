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
        public TestCommand(Item item, Action toggleCanTest) : base(item)
        {
            ToggleCanTest = toggleCanTest;
        }

        public Action ToggleCanTest { get; set; }
    }

    public class TestCommandHandler : IHandler<TestCommand>
    {
        private readonly Func<IScheduledJob> _jobFactory;
        private readonly IMessageBox _messageBox;

        public TestCommandHandler(Func<IScheduledJob> jobFactory, IMessageBox messageBox)
        {
            _jobFactory = Preconditions.CheckNotNull(jobFactory, nameof(jobFactory));
            _messageBox = Preconditions.CheckNotNull(messageBox, nameof(messageBox));
        }

        public void Handle(TestCommand command)
        {
            if (command?.Item == null)
                return;

            Validate(command.Item);

            command.ToggleCanTest();

            var job = _jobFactory();

            job.Item = command.Item;

            Task.Run(job.ExecuteAsync)
                .ContinueWith(task =>
                {
                    _messageBox.Show("Result: " + job.Item.State.Metadata.DisplayName, "Test", MessageBoxButton.OK, MessageBoxImage.Information);

                    command.ToggleCanTest();
                })
                .ConfigureAwait(false);
        }

        private void Validate(Item item)
        {
            List<ValidationResult> validationResults = null;

            item.Validate(out validationResults);

            if (validationResults.Any())
            {
                ShowValidationErrorsDialog(validationResults);
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

            System.Windows.MessageBox.Show(sb.ToString(), "Validation", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }
    }
}