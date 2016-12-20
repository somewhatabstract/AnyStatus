using System;
using System.ComponentModel;

namespace AnyStatus
{
    [Browsable(false)]
    [DisplayName("Travis CI Build")]
    [Description("")]
    public class TravisCIBuild : Item, IScheduledItem
    {
    }

    public class TravisCIBuildMonitor : IMonitor<TravisCIBuild>
    {
        public void Handle(TravisCIBuild item)
        {
            throw new NotImplementedException();
        }
    }
}
