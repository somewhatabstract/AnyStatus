using System;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Media;

namespace AnyStatus.Models
{
    public class AppVeyorBuild : Item
    {
        [DisplayName("Account Name")]
        public string AccountName { get; set; }

        [DisplayName("Project Name")]
        public string ProjectName { get; set; }

        [DisplayName("API Token")]
        public string ApiToken { get; set; }
    }

    public class AppVeyorBuildHandler : IHandler<AppVeyorBuild>
    {
        public void Handle(AppVeyorBuild item)
        {
            Validate(item);

            var build = GetBuildDetailsAsync(item).Result;

            SetItemColor(item, build);
        }

        private void SetItemColor(AppVeyorBuild item, AppVeyorBuildDetails build)
        {
            switch (build.Status)
            {
                case "success":
                    item.Brush = Brushes.Green;
                    break;

                case "failure":
                    item.Brush = Brushes.Red;
                    break;

                case "queued":
                case "running":
                    item.Brush = Brushes.DodgerBlue;
                    break;

                default:
                    break;
            }
        }

        private async Task<AppVeyorBuildDetails> GetBuildDetailsAsync(AppVeyorBuild item)
        {
            const string host = @"https://ci.appveyor.com/api/projects";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", item.ApiToken);

                var apiUrl = $"{host}/{item.AccountName}/{item.ProjectName}";

                var response = await client.GetAsync(apiUrl);

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();

                var buildResponse = new JavaScriptSerializer().Deserialize<AppVeyorBuildResponse>(content);

                return buildResponse.Build;
            }
        }

        private static void Validate(AppVeyorBuild item)
        {
            if (item == null || string.IsNullOrEmpty(item.ApiToken))
            {
                throw new InvalidOperationException("Invalid item.");
            }
        }
    }

    public class AppVeyorBuildResponse
    {
        public AppVeyorBuildDetails Build { get; set; }
    }

    public class AppVeyorBuildDetails
    {
        public string Status { get; set; }
    }
}
