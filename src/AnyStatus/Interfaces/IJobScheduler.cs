﻿using AnyStatus.Models;

namespace AnyStatus
{
    public interface IJobScheduler
    {
        void Schedule(Item item, bool includeChildren = false);

        void Reschedule(Item item, bool includeChildren = false);

        void Execute(Item item);
    }
}