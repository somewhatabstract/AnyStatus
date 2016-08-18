using System;

namespace AnyStatus.Models
{
    public class Ping : Item
    {
    }

    public class PingHandler : IHandler<Ping>
    {
        public void Handle(Ping item)
        {
            
        }
    }
}
