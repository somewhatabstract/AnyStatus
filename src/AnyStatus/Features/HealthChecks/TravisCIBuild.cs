using System;
using System.ComponentModel;

namespace AnyStatus.Models
{
    [DisplayName("Travis CI Build")]
    [Description("Not implemented.")]
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
