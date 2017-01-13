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
    [Browsable(false)]
    [DisplayName("Uptime Robot")]
    [DisplayColumn("General")]
    public class Pingdom : Item, IScheduledItem
    {
        private const string Category = "Uptime Robot";

        [Required]
        [DisplayName("API Key")]
        [Category(Category)]
        [Description("Required. Uptime Robot API key. You can get the key from \"My Settings\" page.")]
        public string ApiKey { get; set; }

        [DisplayName("Monitor Name")]
        [Category(Category)]
        [Description("Optional. Leave empty for overall status.")]
        public string MonitorName { get; set; }
    }

    public class PingdomMonitor : IMonitor<Pingdom>
    {
        private const string UptimeRobotApi = "https://api.uptimerobot.com";
        private const string ConstApiParams = "format=json&noJsonCallback=1";

        private readonly ILogger _logger;

        public PingdomMonitor(ILogger logger)
        {
            _logger = Preconditions.CheckNotNull(logger, nameof(logger));
        }

        [DebuggerStepThrough]
        public void Handle(Pingdom pingdom)
        {
            
        }
    }
}
