using System;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus.Models
{
    public class TfsBuild : Item
    {
        public TfsBuild()
        {
            Collection = "DefaultCollection";
        }

        [PropertyOrder(1)]
        [Description("Visual Studio Team Services account ({account}.visualstudio.com) or TFS server ({server:port}).")]
        public string Host { get; set; }

        [PropertyOrder(2)]
        [Description()]
        public string Collection { get; set; }

        [PropertyOrder(3)]
        [DisplayName("Team Project")]
        [Description()]
        public string TeamProject { get; set; }

        [PropertyOrder(4)]
        [DisplayName("Build Definition")]
        [Description()]
        public string BuildDefinition { get; set; }

        [PropertyOrder(5)]
        [DisplayName("User Name")]
        public string UserName { get; set; }

        [PropertyOrder(6)]
        [PasswordPropertyText(true)]
        public string Password { get; set; }
    }

    public class TfsBuildHandler : IHandler<TfsBuild>
    {
        public void Handle(TfsBuild item)
        {
            Validate(item);

            var buildId = GetBuildDefinitionIdAsync(item).Result;

            //var build = GetBuildDetailsAsync(item).Result;
        }

        private async Task<int> GetBuildDefinitionIdAsync(TfsBuild item)
        {
            using (var handler = new WebRequestHandler { UseDefaultCredentials = true })
            using (var client = new HttpClient(handler))
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                if (!string.IsNullOrEmpty(item.UserName) && !string.IsNullOrEmpty(item.Password))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                        Convert.ToBase64String(Encoding.ASCII.GetBytes($"{item.UserName}:{item.Password}")));
                }

                var url = $"{item.Host}/{item.Collection}/{item.TeamProject}/_apis/build/definitions?api-version=2.0&name={item.BuildDefinition}";

                var response = await client.GetAsync(url);

                var content = await response.Content.ReadAsStringAsync();

                return 0;
            }
        }

        private async Task<TfsBuildDetails> GetBuildDetailsAsync(TfsBuild item)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json-patch+json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", item.UserName, item.Password))));

                var apiUrl = $"{item.Host}/{item.Collection}/{item.TeamProject}/_apis/build/builds?definitions={item.BuildDefinition}&statusFilter=completed&$top=1&api-version=2.0";

                var response = await client.GetAsync(apiUrl);

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();

                var buildResponse = new JavaScriptSerializer().Deserialize<TfsBuildDetails>(content);

                return null;
            }
        }

        private void Validate(TfsBuild item)
        {
            if (item == null || item.Host == null)
            {
                throw new InvalidOperationException("Invalid item.");
            }
        }
    }

    public class TfsBuildDetails
    {

    }
}
