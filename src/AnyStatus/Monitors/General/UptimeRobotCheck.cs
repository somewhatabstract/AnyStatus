using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Web.Script.Serialization;

namespace AnyStatus
{
    [DisplayName("Uptime Robot")]
    [DisplayColumn("General")]
    public class UptimeRobot : Item, IScheduledItem
    {
        private const string Category = "Uptime Robot";

        [Required]
        [DisplayName("Api Key")]
        [Category(Category)]
        [Description("Required.")]
        public string ApiKey { get; set; }

        [DisplayName("Monitor Name")]
        [Category(Category)]
        [Description("Optional. Leave empty for overall status.")]
        public string MonitorName { get; set; }

        [Browsable(false)]
        public bool IsSingleMode { get { return !string.IsNullOrEmpty(MonitorName); } }
    }

    public class UptimeRobotMonitor : IMonitor<UptimeRobot>
    {
        private readonly ILogger _logger;

        public UptimeRobotMonitor(ILogger logger)
        {
            _logger = Preconditions.CheckNotNull(logger, nameof(logger));
        }

        [DebuggerStepThrough]
        public void Handle(UptimeRobot uptimeRobot)
        {
            Status status;

            if (uptimeRobot.IsSingleMode)
            {
                var monitor = GetMonitor(uptimeRobot);

                status = monitor.status;
            }
            else
            {
                var monitors = GetMonitors(uptimeRobot);

                status = monitors.Max(k => k.status);
            }

            switch (status)
            {
                case Status.Pause:
                    uptimeRobot.State = State.Disabled;
                    break;
                case Status.NotChecked:
                    uptimeRobot.State = State.None;
                    break;
                case Status.Up:
                    uptimeRobot.State = State.Ok;
                    break;
                case Status.SeemsDown:
                case Status.Down:
                    uptimeRobot.State = State.Failed;
                    break;
                default:
                    uptimeRobot.State = State.Unknown;
                    break;
            }
        }

        private static Monitor GetMonitor(UptimeRobot uptimeRobot)
        {
            var monitorsResponse = GetMonitorsResponse(uptimeRobot.ApiKey, uptimeRobot.MonitorName);

            if (monitorsResponse.stat == "fail")
            {
                throw new Exception($"\"{uptimeRobot.Name}\" failed to update. Uptime Robot: {monitorsResponse.message}.");
            }

            return monitorsResponse.monitors.monitor.First();
        }

        private static List<Monitor> GetMonitors(UptimeRobot uptimeRobot)
        {
            var total = 1;
            var monitors = new List<Monitor>();

            while (monitors.Count < total)
            {
                var monitorsResponse = GetMonitorsResponse(uptimeRobot.ApiKey, monitors.Count);

                if (monitorsResponse.stat == "fail")
                    throw new Exception($"\"{uptimeRobot.Name}\" failed to update. Uptime Robot: {monitorsResponse.message}.");

                monitors.AddRange(monitorsResponse.monitors.monitor);

                total = monitorsResponse.total;
            }

            return monitors;
        }

        private static MonitorsResponse GetMonitorsResponse(string apiKey, string monitorName)
        {
            var uri = $"https://api.uptimerobot.com/getMonitors?apiKey={apiKey}&format=json&noJsonCallback=1&limit=1&search={monitorName}";

            using (var client = new HttpClient())
            {
                var response = client.GetAsync(uri).Result;

                response.EnsureSuccessStatusCode();

                var content = response.Content.ReadAsStringAsync().Result;

                return new JavaScriptSerializer().Deserialize<MonitorsResponse>(content);
            }
        }

        private static MonitorsResponse GetMonitorsResponse(string apiKey, int offset)
        {
            var uri = $"https://api.uptimerobot.com/getMonitors?apiKey={apiKey}&format=json&noJsonCallback=1&offset={offset}";

            using (var client = new HttpClient())
            {
                var response = client.GetAsync(uri).Result;

                response.EnsureSuccessStatusCode();

                var content = response.Content.ReadAsStringAsync().Result;

                return new JavaScriptSerializer().Deserialize<MonitorsResponse>(content);
            }
        }

        #region Contracts

        public class Monitor
        {
            public Status status { get; set; }
        }

        public class Monitors
        {
            public List<Monitor> monitor { get; set; }
        }

        public class MonitorsResponse
        {
            public string stat { get; set; }
            public int total { get; set; }
            public string message { get; set; }
            public Monitors monitors { get; set; }
        }

        public enum Status
        {
            Pause = 0,
            NotChecked = 1,
            Up = 2,
            SeemsDown = 8,
            Down = 9
        }

        #endregion
    }
}
