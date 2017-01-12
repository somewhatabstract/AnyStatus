using System;
using System.Threading.Tasks;

namespace AnyStatus
{
    public class ScheduledJob : IScheduledJob
    {
        private ILogger _logger;
        private IMediator _mediator;

        public ScheduledJob(ILogger logger, IMediator mediator)
        {
            _logger = Preconditions.CheckNotNull(logger, nameof(logger));
            _mediator = Preconditions.CheckNotNull(mediator, nameof(mediator));
        }

        public Item Item { get; set; }

        public void Execute()
        {
            if (CanExecute() == false)
                return;

            try
            {
                _logger.Info($"Updating \"{Item.Name}\".");

                _mediator.Send(Item, typeof(IMonitor<>));
            }
            catch (Exception ex)
            {
                _logger.Error(ex.InnerException, $"An error occurred while updating \"{Item.Name}\"");

                if (Item.State != State.Disabled)
                    Item.State = State.Error;
            }
        }

        public async Task ExecuteAsync()
        {
            await Task.Run(() => Execute());
        }

        private bool CanExecute()
        {
            if (Item == null)
                throw new ArgumentNullException(nameof(Item));

            if (Item.IsValid())
                return true;

            if (Item.IsDisabled)
                return false;

            Item.State = State.Invalid;

            _logger.Info($"\"{Item.Name}\" is invalid. Make sure all required fields are valid.");

            return false;
        }
    }
}
