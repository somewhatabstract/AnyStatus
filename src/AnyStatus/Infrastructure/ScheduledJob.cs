using AnyStatus.Interfaces;
using AnyStatus.Models;
using FluentScheduler;
using System;
using System.Windows.Media;

namespace AnyStatus.Infrastructure
{
    public class ScheduledJob : IJob
    {
        private ILogger _logger;

        public ScheduledJob(ILogger logger)
        {
            _logger = logger;
        }

        public Item Item
        {
            get; set;
        }

        public void Execute()
        {
            if (Item == null)
                throw new InvalidOperationException("Item cannot be null.");

            try
            {
                _logger.Log("Executing job " + Item.Name);

                var handlerType = typeof(IHandler<>);
                var genericHandlerType = handlerType.MakeGenericType(Item.GetType());
                var handler = TinyIoCContainer.Current.Resolve(genericHandlerType);
                genericHandlerType.GetMethod("Handle").Invoke(handler, new[] { Item });
            }
            catch (Exception ex)
            {
                _logger.Log("Failed to execute job " + Item.Name + ". Exception: " + ex.ToString());

                if (Item != null)
                    Item.Brush = Brushes.Silver;
            }
        }
    }
}
