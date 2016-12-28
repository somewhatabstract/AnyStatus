using System;
using System.ComponentModel;
using System.Diagnostics;

namespace AnyStatus.Features.Monitors
{
    [Browsable(false)]
    [DisplayName("Coveralls")]
    [Description("Shows the percentage of code coverage")]
    public class Coveralls : Metric, IScheduledItem
    {
        public string Url { get; set; }
    }

    public class CoverallsMonitor : IMonitor<Coveralls>
    {
        [DebuggerStepThrough]
        public void Handle(Coveralls item)
        {
            //https://coveralls.io/github/AlonAm/AnyStatus.json?branch=master
        }

        class CoverallsResponse
        {
            public string covered_percent { get; set; }
        }
    }
}
