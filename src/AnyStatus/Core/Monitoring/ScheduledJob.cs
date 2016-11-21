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

            //todo: separate concerns to layers

            try
            {
                if (Item.IsValid())
                {
                    _logger.Info($"Updating \"{Item.Name}\".");

                    _mediator.Send(Item, typeof(IHandler<>));
                }
                else
                {
                    Item.State = State.Invalid;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Failed to update \"{Item.Name}\".");

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
