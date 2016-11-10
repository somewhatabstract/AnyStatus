using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Net.Sockets;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus.Models
{
    [DisplayName("TCP")]
    [Description("")]
    public class TcpPort : Item, IScheduledItem
    {
        [Required]
        [Category("TCP")]
        [PropertyOrder(10)]
        [Description("Host Name or IP Address")]
        public string Host { get; set; }

        [Required]
        [Category("TCP")]
        [PropertyOrder(20)]
        [Range(0, ushort.MaxValue, ErrorMessage = "Port must be between 0 and 65535")]
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

                    item.State = State.Ok;
                }
                catch (SocketException)
                {
                    item.State = State.Failed;
                }
            }
        }
    }
}
