using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace AnyStatus
{
    [DisplayName("Windows Service")]
    [Description("")]
    public class WindowsService : Item, IScheduledItem, ICanStartWindowsService, ICanStopWindowsService, ICanRestartWindowsService
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

    public abstract class BaseWindowsServiceHandler
    {
        protected static TimeSpan Timeout = TimeSpan.FromMinutes(1);

        protected ServiceController GetServiceController(WindowsService windowsService)
        {
            return string.IsNullOrEmpty(windowsService.MachineName) ?
                    new ServiceController(windowsService.ServiceName) :
                    new ServiceController(windowsService.ServiceName, windowsService.MachineName);
        }

        protected static void SetState(WindowsService windowsService, ServiceController serviceController)
        {
            windowsService.State = serviceController.Status == windowsService.Status ? State.Ok : State.Failed;
        }
    }

    public class WindowsServiceMonitor : BaseWindowsServiceHandler, IMonitor<WindowsService>
    {
        [DebuggerStepThrough]
        public void Handle(WindowsService windowsService)
        {
            using (var sc = GetServiceController(windowsService))
            {
                SetState(windowsService, sc);

                sc.Close();
            }
        }
    }

    public class StartWindowsService : BaseWindowsServiceHandler, IStartWindowsService<WindowsService>
    {
        public async Task HandleAsync(WindowsService windowsService)
        {
            await Task.Run(() =>
            {
                using (var sc = GetServiceController(windowsService))
                {
                    if (sc.Status != ServiceControllerStatus.Running && sc.Status != ServiceControllerStatus.StartPending)
                        sc.Start();

                    sc.WaitForStatus(ServiceControllerStatus.Running, Timeout);

                    SetState(windowsService, sc);

                    sc.Close();
                }
            });
        }
    }

    public class StopWindowsService : BaseWindowsServiceHandler, IStopWindowsService<WindowsService>
    {
        public async Task HandleAsync(WindowsService windowsService)
        {
            await Task.Run(() =>
            {
                using (var sc = GetServiceController(windowsService))
                {
                    if (sc.Status == ServiceControllerStatus.Running || sc.Status == ServiceControllerStatus.StartPending)
                        sc.Stop();

                    sc.WaitForStatus(ServiceControllerStatus.Stopped, Timeout);

                    SetState(windowsService, sc);

                    sc.Close();
                }
            });
        }
    }

    public class RestartWindowsService : BaseWindowsServiceHandler, IRestartWindowsService<WindowsService>
    {
        public async Task HandleAsync(WindowsService windowsService)
        {
            await Task.Run(() =>
            {
                using (var sc = GetServiceController(windowsService))
                {
                    if (sc.Status == ServiceControllerStatus.Running || sc.Status == ServiceControllerStatus.StartPending)
                        sc.Stop();

                    sc.WaitForStatus(ServiceControllerStatus.Stopped, Timeout);

                    SetState(windowsService, sc);

                    sc.Start();

                    sc.WaitForStatus(ServiceControllerStatus.Running, Timeout);

                    SetState(windowsService, sc);

                    sc.Close();
                }
            });
        }
    }
}