using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Media;

namespace AnyStatus.Models
{
    [DisplayName("AppVeyor Build")]
    [Description("")]
    public class AppVeyorBuild : Item, IScheduledItem
    {
        [Required]
        [Category("AppVeyor")]
        [DisplayName("Account Name")]
        [Description("The AppVeyor account name")]
        public string AccountName { get; set; }

        [Browsable(false)]
        public string ProjectName { get; set; } //Obsolete. Remove in future versions.

        [Required]
        [Category("AppVeyor")]
        [DisplayName("Project Slug")]
        [Description("The project slug is the last part of the AppVeyor project URL. For example: https://ci.appveyor.com/project/AccountName/Project-Slug")]
        public string ProjectSlug { get; set; }

        [Required]
        [Category("AppVeyor")]
        [DisplayName("API Token")]
        [Description("The AppVeyor API token")]
        public string ApiToken { get; set; }
    }

    public class AppVeyorBuildHandler : IHandler<AppVeyorBuild>
    {
        [DebuggerStepThrough]
        public void Handle(AppVeyorBuild item)
        {
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

                var apiUrl = $"{host}/{item.AccountName}/{item.ProjectSlug}";

                var response = await client.GetAsync(apiUrl);

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();

                var buildResponse = new JavaScriptSerializer().Deserialize<AppVeyorBuildResponse>(content);

                return buildResponse.Build;
            }
        }

        #region Contracts

        public class AppVeyorBuildResponse
        {
            public AppVeyorBuildDetails Build { get; set; }
        }

        public class AppVeyorBuildDetails
        {
            public string Status { get; set; }
        }

        #endregion
    }
}
