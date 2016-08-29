using System.ComponentModel;
using System.Net.Sockets;
using System.Windows.Media;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus.Models
{
    [DisplayName("TCP Port")]
    [Description("")]
    public class TcpPort : Item
    {
        [PropertyOrder(1)]
        [Description("Host Name or IP Address")]
        public string Host { get; set; }

        [PropertyOrder(2)]
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
