using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Windows.Media;

namespace AnyStatus.Models
{
    [DisplayName("Ping")]
    [Description("")]
    public class Ping : Item, IScheduledItem
    {
        [Required]
        [Description("Host Name or IP Address")]
        public string Host { get; set; }
    }

    public class PingHandler : IHandler<Ping>
    {
        [DebuggerStepThrough]
        public void Handle(Ping item)
        {
            using (var ping = new System.Net.NetworkInformation.Ping())
            {
                var reply = ping.Send(item.Host);

                item.Brush = reply.Status == IPStatus.Success ? Brushes.Green : Brushes.Red;
            }
        }
    }
}
