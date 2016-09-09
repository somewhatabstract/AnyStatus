using System.ComponentModel;

namespace AnyStatus.Models
{
    [Browsable(false)]
    [DisplayName("PowerShell")]
    [Description("Execute a PowerShell script")]
    public class PowerShell : Item, IScheduledItem
    {
    }

    public class PowerShellHandler : IHandler<PowerShell>
    {
        public void Handle(PowerShell item)
        {
            //using (PowerShell PowerShellInstance = PowerShell.Create())
            //{
            //}
        }
    }
}
