using AnyStatus.Interfaces;
using AnyStatus.Models;
using FluentScheduler;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AnyStatus.Infrastructure
{
    public class ItemRegistry : Registry
    {
        public ItemRegistry(IUserSettings userSettings)
        {
            try
            {
                if (userSettings.Items != null)
                {
                    ScheduleJobs(userSettings.Items);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private void ScheduleJobs(IEnumerable<Item> items)
        {
            foreach (var item in items)
            {
                //todo: mark schedule jobs with interface

                if (item is Folder && item.Items != null)
                {
                    ScheduleJobs(item.Items);

                    continue;
                }

                Action action = () =>
                {
                    Debug.WriteLine(DateTime.Now + " handling " + item.Name);

                    try
                    {
                        //Mediator
                        var a = typeof(IHandler<>);
                        var b = a.MakeGenericType(item.GetType());
                        var handler = TinyIoCContainer.Current.Resolve(b);
                        b.GetMethod("Handle").Invoke(handler, new[] { item });
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                };

                Schedule(action)
                    .NonReentrant()
                     .WithName(item.Id.ToString())
                     .ToRunNow()
                     .AndEvery(5).Seconds();
            }
        }
    }
}
