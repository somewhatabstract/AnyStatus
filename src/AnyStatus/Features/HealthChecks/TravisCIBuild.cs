using System;
using System.ComponentModel;

namespace AnyStatus.Models
{
    [Browsable(false)]
    [DisplayName("Travis CI Build")]
    [Description("")]
    public class TravisCIBuild : Item
    {
    }

    public class TravisCIBuildHandler : IHandler<TravisCIBuild>
    {
        public void Handle(TravisCIBuild item)
        {
            throw new NotImplementedException();
        }
    }
}
