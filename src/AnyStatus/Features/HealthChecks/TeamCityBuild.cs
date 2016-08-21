using System;
using System.ComponentModel;

namespace AnyStatus.Models
{
    public class TeamCityBuild : Item
    {
        [Description("TeamCity server host name or IP address.")]
        public string Host { get; set; }

        public string BuildTypeId { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }
    }

    //https://teamcity.jetbrains.com/app/rest/builds?running:any&locator=buildType:(id:Checkstyle_IdeaInspectionsPullRequest),count:1&fields=build(number,status,running)
    //https://teamcity.jetbrains.com/app/rest/builds?running:any&locator=buildType:(id:MPS_34_Distribution_TestMbeddrBuild),count:1&fields=build(number,status,running)
    //https://teamcity.jetbrains.com/httpAuth/app/rest/buildTypes/id:NetCommunityProjects_Femah_Commit/builds/running:any/status
    //https://teamcity.jetbrains.com/httpAuth/app/rest/buildTypes/id:MPS_34_Distribution_TestMbeddrBuild/builds/running:any/state

    public class TeamCityBuildHandler : IHandler<TeamCityBuild>
    {
        public void Handle(TeamCityBuild item)
        {
            throw new NotImplementedException();
        }
    }
}
