using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Media;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus.Models
{
    public class TeamCityBuild : Item
    {
        [PropertyOrder(1)]
        [Description("TeamCity server host name or IP address.")]
        public string Host { get; set; }

        [PropertyOrder(2)]
        public string BuildTypeId { get; set; }

        [PropertyOrder(3)]
        public bool GuestUser { get; set; }

        [PropertyOrder(4)]
        public string UserName { get; set; }

        [PropertyOrder(5)]
        [PasswordPropertyText]
        public string Password { get; set; }

        [PropertyOrder(6)]
        [DisplayName("Ignore SSL Errors")]
        public bool IgnoreSslErrors { get; set; }
    }

    public class TeamCityBuildHandler : IHandler<TeamCityBuild>
    {
        public void Handle(TeamCityBuild item)
        {
            Validate(item);

            var build = GetBuildDetailsAsync(item).Result;

            SetItemColor(item, build);
        }

        private async Task<TeamCityBuildDetails> GetBuildDetailsAsync(TeamCityBuild item)
        {
            string authType = string.Empty;

            using (var handler = new WebRequestHandler())
            {
                if (item.IgnoreSslErrors)
                {
                    handler.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                }

                if (item.GuestUser)
                {
                    authType = "guestAuth";
                }
                else
                {
                    authType = "httpAuth";
                    handler.Credentials = new NetworkCredential(item.UserName, item.Password);
                }

                using (var client = new HttpClient(handler))
                {
                    client.DefaultRequestHeaders.Add("Accept", "application/json");

                    var apiUrl = $"{item.Host}/{authType}/app/rest/builds?locator=running:any,buildType:(id:{item.BuildTypeId}),count:1&fields=build(status,running)";

                    var response = await client.GetAsync(apiUrl);

                    response.EnsureSuccessStatusCode();

                    var content = await response.Content.ReadAsStringAsync();

                    var buildResponse = new JavaScriptSerializer().Deserialize<TeamCityBuildDetailsResponse>(content);

                    return buildResponse.Build[0];
                }
            }
        }

        private void SetItemColor(TeamCityBuild item, TeamCityBuildDetails build)
        {
            if (build.Running)
            {
                item.Brush = Brushes.DodgerBlue;
                return;
            }

            switch (build.Status)
            {
                case "SUCCESS":
                    item.Brush = Brushes.Green;
                    break;

                case "FAILURE":
                    item.Brush = Brushes.Red;
                    break;

                default:
                    break;
            }
        }

        private static void Validate(TeamCityBuild item)
        {
            if (item == null || string.IsNullOrEmpty(item.Host) || string.IsNullOrEmpty(item.BuildTypeId))
            {
                throw new InvalidOperationException("Invalid item.");
            }
        }
    }

    public class TeamCityBuildDetailsResponse
    {
        public List<TeamCityBuildDetails> Build { get; set; }
    }

    public class TeamCityBuildDetails
    {
        public bool Running { get; set; }

        public string Status { get; set; }
    }
}
