using System;

namespace AnyStatus.Models
{
    public class WindowsService : Item
    {
    }

    public class WindowsServiceHandler : IHandler<WindowsService>
    {
        public void Handle(WindowsService item)
        {
            throw new NotImplementedException();
        }
    }
}
