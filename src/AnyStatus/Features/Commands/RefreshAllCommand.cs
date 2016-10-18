﻿using System;

namespace AnyStatus
{
    public class RefreshAllCommand : BaseCommand
    {
        private ILogger _logger;
        private IJobScheduler _jobScheduler;

        public RefreshAllCommand(IJobScheduler jobScheduler, ILogger logger) :
            base(PackageIds.refreshToolbarCommandId)
        {
            _jobScheduler = jobScheduler;
            _logger = logger;
        }

        protected override void Handle(object sender, EventArgs e)
        {
            try
            {
                _jobScheduler.ExecuteAll();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to refresh all items");
            }
        }
    }
}
