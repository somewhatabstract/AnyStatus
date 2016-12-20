using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.ServiceProcess;

namespace AnyStatus
{
    [DisplayName("Windows Service")]
    [Description("")]
    public class WindowsService : Item, IScheduledItem
    {
        public WindowsService()
        {
            Status = ServiceControllerStatus.Running;
        }

        [Required]
        [Category("Windows Service")]
        [DisplayName("Service Name")]
        public string ServiceName { get; set; }

        [DisplayName("Machine Name")]
        [Category("Windows Service")]
        [Description("The machine name (optional). Leave blank for local computer")]
        public string MachineName { get; set; }

        [Category("Windows Service")]
        public ServiceControllerStatus Status { get; set; }
    }

    public class WindowsServiceMonitor : IMonitor<WindowsService>
    {
        [DebuggerStepThrough]
        public void Handle(WindowsService item)
        {
            var sc = string.IsNullOrEmpty(item.MachineName) ? 
                new ServiceController(item.ServiceName) : 
                new ServiceController(item.ServiceName, item.MachineName);

            item.State = (sc.Status == item.Status) ? State.Ok : State.Failed;

            sc.Dispose();
        }
    }
}
