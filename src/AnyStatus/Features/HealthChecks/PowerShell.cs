using System;
using System.ComponentModel;

namespace AnyStatus.Models
{
    [Browsable(false)]
    [DisplayName("PowerShell")]
    [Description("")]
    public class PowerShell : Item, IScheduledItem
    {
    }

    public class PowerShellHandler : IHandler<PowerShell>
    {
        public void Handle(PowerShell item)
        {
            throw new NotImplementedException();
        }
    }
}
