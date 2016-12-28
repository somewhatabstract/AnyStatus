﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;

namespace AnyStatus
{
    [DisplayName("AppVeyor Build")]
    [Description("AppVeyor build status")]
    [DisplayColumn("Continuous Integration")]
    public class AppVeyorBuild : Item, IScheduledItem, ICanOpenInBrowser, ICanTriggerBuild
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

    public class AppVeyorBuildMonitor : IMonitor<AppVeyorBuild>
    {
        [DebuggerStepThrough]
        public void Handle(AppVeyorBuild item)
        {
            var build = GetBuildDetailsAsync(item).Result;

            switch (build.Status)
            {
                case "success":
                    item.State = State.Ok;
                    break;
                case "failed":
                case "failure":
                    item.State = State.Failed;
                    break;
                case "queued":
                case "running":
                    item.State = State.Running;
                    break;
                default:
                    item.State = State.Unknown;
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

    public class TriggerAppVeyorBuild : ITriggerBuild<AppVeyorBuild>
    {
        const string Url = @"https://ci.appveyor.com/api/builds";

        private readonly ILogger _logger;
        private readonly IDialogService _dialogService;

        public TriggerAppVeyorBuild(IDialogService dialogService, ILogger logger)
        {
            _logger = Preconditions.CheckNotNull(logger, nameof(logger));
            _dialogService = Preconditions.CheckNotNull(dialogService, nameof(dialogService));
        }

        public async Task HandleAsync(AppVeyorBuild build)
        {
            var result = _dialogService.Show($"Are you sure you want to trigger {build.Name}?", "Trigger a new build", MessageBoxButton.YesNo, MessageBoxImage.Asterisk);

            if (result != MessageBoxResult.Yes)
                return;

            await QueueNewBuild(build);

            _logger.Info($"Build \"{build.Name}\" was triggered.");
        }

        private async Task QueueNewBuild(AppVeyorBuild item)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", item.ApiToken);

                var request = new
                {
                    accountName = item.AccountName,
                    projectSlug = item.ProjectSlug
                };

                var data = new JavaScriptSerializer().Serialize(request);

                var content = new StringContent(data, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(Url, content);

                response.EnsureSuccessStatusCode();
            }
        }
    }

    public class OpenAppVeyorBuildInBrowser : IOpenInBrowser<AppVeyorBuild>
    {
        private readonly IProcessStarter _processStarter;

        public OpenAppVeyorBuildInBrowser(IProcessStarter processStarter)
        {
            _processStarter = Preconditions.CheckNotNull(processStarter, nameof(processStarter));
        }

        public void Handle(AppVeyorBuild item)
        {
            if (item == null || item.IsValid() == false)
                return;

            var url = $"https://ci.appveyor.com/project/{item.AccountName}/{item.ProjectSlug}";

            _processStarter.Start(url);
        }
    }
}
