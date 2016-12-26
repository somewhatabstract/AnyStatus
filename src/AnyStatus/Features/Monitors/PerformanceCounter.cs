using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Threading;

namespace AnyStatus
{
    [DisplayName("Performance Counter")]
    [Description("Experimental.")]
    public class PerformanceCounter : Metric, IScheduledItem
    {
        private const string Category = "Performance Counter";

        [DisplayName("Machine Name")]
        [Category(Category)]
        [Description("Optional. Leave blank for local computer.")]
        public string MachineName { get; set; }

        [Required]
        [DisplayName("Category")]
        [Category(Category)]
        [Description("")]
        public string CategoryName { get; set; }

        [Required]
        [DisplayName("Counter")]
        [Category(Category)]
        [Description("")]
        public string CounterName { get; set; }

        [Required]
        [DisplayName("Instance")]
        [Category(Category)]
        [Description("")]
        public string InstanceName { get; set; }
    }

    public class PerformanceCounterMonitor : IMonitor<PerformanceCounter>
    {
        [DebuggerStepThrough]
        public void Handle(PerformanceCounter item)
        {
            if (string.IsNullOrWhiteSpace(item.MachineName))
                item.MachineName = "localhost";

            item.Value = (int)GetValue(item);

            item.State = State.Ok;
        }

        public float GetValue(PerformanceCounter item)
        {
            var counter = new System.Diagnostics.PerformanceCounter
            {
                CategoryName = item.CategoryName,
                CounterName = item.CounterName,
                InstanceName = item.InstanceName,
                MachineName = item.MachineName
            };

            return counter.NextValue();
        }
    }
}