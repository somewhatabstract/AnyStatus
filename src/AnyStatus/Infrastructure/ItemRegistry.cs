using AnyStatus.Interfaces;
using AnyStatus.Models;
using FluentScheduler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Media;

namespace AnyStatus.Infrastructure
{
    public class ItemRegistry : Registry
    {
        public ItemRegistry(IUserSettings userSettings)
        {
            try
            {
                NonReentrantAsDefault();

                Schedule(userSettings.Items);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private void Schedule(IEnumerable<Item> items)
        {
            if (items == null)
            {
                return;
            }

            foreach (var item in items)
            {
                //todo: mark scheduled jobs with interface

                if (item is Folder)
                {
                    Schedule(item.Items);
                    continue;
                }

                if (item.Interval <= 0)
                {
                    continue;
                }

                Schedule(item);
            }
        }

        private void Schedule(Item item)
        {
            Action action = () =>
            {
                try
                {
                    var a = typeof(IHandler<>);
                    var b = a.MakeGenericType(item.GetType());
                    var handler = TinyIoCContainer.Current.Resolve(b);
                    b.GetMethod("Handle").Invoke(handler, new[] { item });
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    item.Brush = Brushes.Silver;
                }
            };

            Schedule(action)
                 .WithName(item.Id.ToString())
                 .ToRunNow()
                 .AndEvery(item.Interval).Minutes();
        }
    }
}
