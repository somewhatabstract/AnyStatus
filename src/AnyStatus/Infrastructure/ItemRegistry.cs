﻿using AnyStatus.Interfaces;
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
            if (items == null) return;

            foreach (var item in items)
            {
                if (item is Folder)
                {
                    Schedule(item.Items);
                }
                else
                {
                    Schedule(item);
                }
            }
        }

        private void Schedule(Item item)
        {
            Schedule(new ScheduledJob(item))
                 .WithName(item.Id.ToString())
                 .ToRunNow()
                 .AndEvery(item.Interval).Minutes();
        }
    }
}
