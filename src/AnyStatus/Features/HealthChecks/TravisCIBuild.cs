using AnyStatus.Models;
using System;

namespace AnyStatus.Features.HealthChecks
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
