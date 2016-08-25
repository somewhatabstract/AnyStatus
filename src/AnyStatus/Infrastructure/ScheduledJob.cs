using AnyStatus.Models;
using FluentScheduler;
using System;
using System.Diagnostics;
using System.Windows.Media;

namespace AnyStatus.Infrastructure
{
    public class ScheduledJob : IJob
    {
        private readonly Item _item;

        public ScheduledJob(Item item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            _item = item;
        }

        public void Execute()
        {
            try
            {
                var handlerType = typeof(IHandler<>);
                var genericHandlerType = handlerType.MakeGenericType(_item.GetType());
                var handler = TinyIoCContainer.Current.Resolve(genericHandlerType);
                genericHandlerType.GetMethod("Handle").Invoke(handler, new[] { _item });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);

                _item.Brush = Brushes.Silver;
            }
        }
    }
}
