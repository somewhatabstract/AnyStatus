using FluentScheduler;
using System;
using System.Threading.Tasks;

namespace AnyStatus
{
    public class ScheduledJob : IJob
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
            if (Item == null)
                throw new InvalidOperationException("Item cannot be null.");

            if (Item.State == State.Disabled)
                return;

            if (Item.IsValid() == false)
            {
                Item.State = State.Invalid;

                _logger.Info($"\"{Item.Name}\" is invalid.");

                return;
            }

            try
            {
                _logger.Info($"Updating \"{Item.Name}\".");

                _mediator.Send(Item);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Failed to update \"{Item.Name}\".");

                if (Item.State != State.Disabled)
                    Item.State = State.Error;
            }
        }

        public async Task ExecuteAsync()
        {
            await Task.Run(() =>
            {
                Execute();
            });
        }
    }
}
