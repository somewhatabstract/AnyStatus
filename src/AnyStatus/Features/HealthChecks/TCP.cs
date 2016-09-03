using System.ComponentModel;
using System.Net.Sockets;
using System.Windows.Media;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using System.ComponentModel.DataAnnotations;

namespace AnyStatus.Models
{
    [DisplayName("TCP")]
    [Description("")]
    public class TcpPort : Item, IScheduledItem
    {
        [Required]
        [PropertyOrder(10)]
        [Description("Host Name or IP Address")]
        public string Host { get; set; }

        [Required]
        [Range(0, ushort.MaxValue, ErrorMessage = "Port must be between 0 and 65535")]
        [PropertyOrder(20)]
        public int Port { get; set; }
    }

    public class TcpPortHandler : IHandler<TcpPort>
    {
        public void Handle(TcpPort item)
        {
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                try
                {
                    socket.Connect(item.Host, item.Port);

                    item.Brush = Brushes.Green;
                }
                catch (SocketException)
                {
                    item.Brush = Brushes.Red;
                }
            }
        }
    }
}
