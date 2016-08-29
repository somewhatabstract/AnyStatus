using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Media;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus.Models
{
    public class TeamCityBuild : Item
    {
        [PropertyOrder(1)]
        [DisplayName("Url")]
        [Description("TeamCity server URL address. For example: http://teamcity:8080 or https://teamcity.jetbrains.com")]
        public string Url { get; set; }

        [Browsable(false)] //TODO: Remove property. Use Url instead.
        public string Host { get { return Url; } set { Url = value; } }

        [PropertyOrder(2)]
        [DisplayName("Build Type Id")]
        [Description("TeamCity build type id (You can copy it from TeamCity build URL address).")]
        public string BuildTypeId { get; set; }

        [PropertyOrder(3)]
        [DisplayName("Guest User")]
        [Description("Use TeamCity guest user to log in.")]
        public bool GuestUser { get; set; }

        [PropertyOrder(4)]
        [DisplayName("User Name")]
        [Description("Optional.")]
        public string UserName { get; set; }

        [PropertyOrder(5)]
        [PasswordPropertyText(true)]
        [Description("Optional.")]
        public string Password { get; set; }

        [PropertyOrder(6)]
        [DisplayName("Ignore SSL Errors")]
        public bool IgnoreSslErrors { get; set; }
    }

    public class TeamCityBuildHandler : IHandler<TeamCityBuild>
    {
        [DebuggerStepThrough]
        public void Handle(TeamCityBuild item)
        {
            Validate(item);

            var build = GetBuildDetailsAsync(item).Result;

            SetItemColor(item, build);
        }

        private async Task<TeamCityBuildDetails> GetBuildDetailsAsync(TeamCityBuild item)
        {
            using (var handler = new WebRequestHandler())
            {
                handler.UseDefaultCredentials = true;

                if (item.IgnoreSslErrors)
                {
                    handler.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                }

                string authType = string.Empty;

                if (item.GuestUser)
                {
                    authType = "guestAuth";
                }
                else
                {
                    authType = "httpAuth";

                    if (!string.IsNullOrEmpty(item.UserName) && !string.IsNullOrEmpty(item.Password))
                    {
                        handler.Credentials = new NetworkCredential(item.UserName, item.Password);
                    }
                }

                using (var client = new HttpClient(handler))
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var apiUrl = $"{item.Url}/{authType}/app/rest/builds?locator=running:any,buildType:(id:{item.BuildTypeId}),count:1&fields=build(status,running)";

                    var response = await client.GetAsync(apiUrl);

                    response.EnsureSuccessStatusCode();

                    var content = await response.Content.ReadAsStringAsync();

                    var buildResponse = new JavaScriptSerializer().Deserialize<TeamCityBuildDetailsResponse>(content);

                    return buildResponse.Build.First();
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

            //if (build.CancelledInfo)
            //{
            //    item.Brush = Brushes.Gray;
            //    return;
            //}

            switch (build.Status)
            {
                case "SUCCESS":
                    item.Brush = Brushes.Green;
                    break;

                case "FAILURE":
                case "ERROR":
                    item.Brush = Brushes.Red;
                    break;

                case "UNKNOWN":
                    item.Brush = Brushes.Gray;
                    break;

                default:
                    break;
            }
        }

        private static void Validate(TeamCityBuild item)
        {
            if (item == null || string.IsNullOrEmpty(item.Url) || string.IsNullOrEmpty(item.BuildTypeId))
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
