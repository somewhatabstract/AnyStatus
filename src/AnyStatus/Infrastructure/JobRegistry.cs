using System;
using System.Collections.ObjectModel;
using AnyStatus.Interfaces;
using AnyStatus.Models;
using FluentScheduler;
using System.Diagnostics;
using System.Collections.Generic;

namespace AnyStatus.Infrastructure
{
    public class JobRegistry : Registry
    {
        public JobRegistry(IUserSettings userSettings)
        {
            try
            {
                var items = userSettings.Items;

                ScheduleJobs(items);
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
                    var a = typeof(IHandler<>);
                    var b = a.MakeGenericType(item.GetType());
                    var handler = TinyIoC.TinyIoCContainer.Current.Resolve(b);
                    b.GetMethod("Handle").Invoke(handler, new[] { item });
                };

                Schedule(action)
                    .NonReentrant()
                     .WithName(item.Id.ToString())
                     .ToRunEvery(5).Seconds();


            }
        }
    }
}
