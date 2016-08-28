using System;

namespace AnyStatus.Models
{
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
