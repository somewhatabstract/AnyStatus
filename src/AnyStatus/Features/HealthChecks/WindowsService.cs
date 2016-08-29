using System.ComponentModel;
using System.ServiceProcess;
using System.Windows.Media;

namespace AnyStatus.Models
{
    [DisplayName("Windows Service")]
    [Description("")]
    public class WindowsService : Item
    {
        public WindowsService()
        {
            Status = ServiceControllerStatus.Running;
        }

        [DisplayName("Service Name")]
        public string ServiceName { get; set; }

        [DisplayName("Machine Name")]
        [Description("Optional. Leave blank for local computer.")]
        public string MachineName { get; set; }

        public ServiceControllerStatus Status { get; set; }
    }

    public class WindowsServiceHandler : IHandler<WindowsService>
    {
        public void Handle(WindowsService item)
        {
            var sc = string.IsNullOrEmpty(item.MachineName) ? 
                new ServiceController(item.ServiceName) : 
                new ServiceController(item.ServiceName, item.MachineName);

            item.Brush = sc.Status == item.Status ? Brushes.Green : Brushes.Red;

            sc.Dispose();
        }
    }
}
