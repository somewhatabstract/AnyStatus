using System;

namespace AnyStatus.Models
{
    public class TeamCityBuild : Item
    {
    }

    public class TeamCityBuildHandler : IHandler<TeamCityBuild>
    {
        public void Handle(TeamCityBuild item)
        {
            throw new NotImplementedException();
        }
    }
}
