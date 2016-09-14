using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Media;
using System.Xml.Serialization;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus.Models
{
    [DisplayName("TFS 2015 Build")]
    [Description("Microsoft Team Foundation Server 2015 or Visual Studio Team Services build status")]
    public class TfsBuild : Item, IScheduledItem
    {
        public TfsBuild()
        {
            Collection = "DefaultCollection";
        }

        [Url]
        [Required]
        [Category("TFS")]
        [PropertyOrder(10)]
        [Description("Visual Studio Team Services account (https://{account}.visualstudio.com) or TFS server (http://{server:port}/tfs)")]
        public string Url { get; set; }

        [Required]
        [Category("TFS")]
        [PropertyOrder(20)]
        [Description()]
        public string Collection { get; set; }

        [Required]
        [Category("TFS")]
        [PropertyOrder(30)]
        [DisplayName("Team Project")]
        [Description()]
        public string TeamProject { get; set; }

        [Required]
        [Category("TFS")]
        [PropertyOrder(40)]
        [DisplayName("Build Definition")]
        [Description()]
        public string BuildDefinition { get; set; }

        [XmlIgnore]
        [Browsable(false)]
        public int BuildDefinitionId { get; set; }

        [PropertyOrder(50)]
        [Category("TFS")]
        [DisplayName("User Name")]
        [Description("The TFS user name (optional)")]
        public string UserName { get; set; }

        [Category("TFS")]
        [PropertyOrder(60)]
        [Description("The TFS password (optional)")]
        [Editor(typeof(PasswordEditor), typeof(PasswordEditor))]
        public string Password { get; set; }
    }

    public class TfsBuildHandler : IHandler<TfsBuild>
    {
        [DebuggerStepThrough]
        public void Handle(TfsBuild item)
        {
            Validate(item);

            if (item.BuildDefinitionId <= 0)
            {
                item.BuildDefinitionId = GetBuildDefinitionIdAsync(item).Result;
            }

            var buildDetails = GetBuildDetailsAsync(item).Result;

            SetItemColor(item, buildDetails);
        }

        private void SetItemColor(TfsBuild item, TfsBuildDetails buildDetails)
        {
            if (buildDetails.Status == "inProgress")
            {
                item.Brush = Brushes.DodgerBlue;
                return;
            }

            switch (buildDetails.Result)
            {
                case "notStarted":
                    item.Brush = Brushes.DodgerBlue;
                    break;

                case "succeeded":
                    item.Brush = Brushes.Green;
                    break;

                case "failed":
                    item.Brush = Brushes.Red;
                    break;

                case "partiallySucceeded":
                    item.Brush = Brushes.Orange;
                    break;

                case "canceled ":
                    item.Brush = Brushes.Gray;
                    break;

                default:
                    break;
            }
        }

        private async Task<int> GetBuildDefinitionIdAsync(TfsBuild item)
        {
            var useDefaultCredentials = string.IsNullOrEmpty(item.UserName) && string.IsNullOrEmpty(item.Password);

            using (var handler = new WebRequestHandler())
            {
                handler.UseDefaultCredentials = useDefaultCredentials;

                using (var client = new HttpClient(handler))
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    if (!useDefaultCredentials)
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                            Convert.ToBase64String(Encoding.ASCII.GetBytes($"{item.UserName}:{item.Password}")));
                    }

                    var url = $"{item.Url}/{item.Collection}/{item.TeamProject}/_apis/build/definitions?api-version=2.0&name={item.BuildDefinition}";

                    var response = await client.GetAsync(url);

                    response.EnsureSuccessStatusCode();

                    var content = await response.Content.ReadAsStringAsync();

                    var buildDefinitionResponse = new JavaScriptSerializer().Deserialize<BuildDefinitionResponse>(content);

                    return buildDefinitionResponse.Value.First().Id;
                }
            }
        }

        private async Task<TfsBuildDetails> GetBuildDetailsAsync(TfsBuild item)
        {
            var useDefaultCredentials = string.IsNullOrEmpty(item.UserName) && string.IsNullOrEmpty(item.Password);

            using (var handler = new WebRequestHandler())
            {
                handler.UseDefaultCredentials = useDefaultCredentials;

                using (var client = new HttpClient(handler))
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    if (!useDefaultCredentials)
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                            Convert.ToBase64String(Encoding.ASCII.GetBytes($"{item.UserName}:{item.Password}")));
                    }

                    var url = $"{item.Url}/{item.Collection}/{item.TeamProject}/_apis/build/builds?definitions={item.BuildDefinitionId}&$top=1&api-version=2.0";

                    var response = await client.GetAsync(url);

                    response.EnsureSuccessStatusCode();

                    var content = await response.Content.ReadAsStringAsync();

                    var buildDetailsResponse = new JavaScriptSerializer().Deserialize<TfsBuildDetailsResponse>(content);

                    return buildDetailsResponse.Value.First();
                }
            }
        }

        private void Validate(TfsBuild item)
        {
            if (item == null || item.Url == null)
            {
                throw new InvalidOperationException("Invalid item.");
            }
        }

        #region Contracts

        private class BuildDefinitionResponse
        {
            public List<BuildDefinitionDetails> Value { get; set; }
        }

        private class BuildDefinitionDetails
        {
            public int Id { get; set; }
        }

        private class TfsBuildDetailsResponse
        {
            public List<TfsBuildDetails> Value { get; set; }
        }

        private class TfsBuildDetails
        {
            public string Result { get; set; }

            public string Status { get; set; }
        }

        #endregion
    }
}
