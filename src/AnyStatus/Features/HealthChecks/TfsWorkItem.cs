using System;
using System.ComponentModel;

namespace AnyStatus.Models
{
    [Browsable(false)]
    [DisplayName("TFS 15 Work Item")]
    [Description("")]
    public class TfsWorkItem : Item, IScheduledItem
    {
    }

    public class TfsWorkItemHandler : IHandler<TfsWorkItem>
    {
        public void Handle(TfsWorkItem item)
        {
            throw new NotImplementedException();
        }
    }
}
