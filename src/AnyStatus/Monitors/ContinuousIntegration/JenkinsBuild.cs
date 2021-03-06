﻿using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus
{
    [DisplayName("Jenkins Build")]
    [Description("Jenkins build status")]
    [DisplayColumn("Continuous Integration")]
    public class JenkinsBuild : Item, IScheduledItem, ICanOpenInBrowser, ICanTriggerBuild
    {
        [Url]
        [Required]
        [PropertyOrder(10)]
        [Category("Jenkins")]
        [Description("Jenkins build URL address")]
        public string Url { get; set; }

        [PropertyOrder(20)]
        [Category("Jenkins")]
        [DisplayName("User Name")]
        [Description("The Jenkins user name (optional)")]
        public string UserName { get; set; }

        [PropertyOrder(30)]
        [Category("Jenkins")]
        [DisplayName("API Token")]
        [Description("The Jenkins API token (optional)")]
        public string ApiToken { get; set; }

        [PropertyOrder(40)]
        [DisplayName("Ignore SSL Errors")]
        public bool IgnoreSslErrors { get; set; }
    }

    public class OpenJenkinsBuildInBrowser : IOpenInBrowser<JenkinsBuild>
    {
        private readonly IProcessStarter _processStarter;

        public OpenJenkinsBuildInBrowser(IProcessStarter processStarter)
        {
            _processStarter = Preconditions.CheckNotNull(processStarter, nameof(processStarter));
        }

        public void Handle(JenkinsBuild item)
        {
            _processStarter.Start(item.Url);
        }
    }

    public class TriggerJenkinsBuild : ITriggerBuild<JenkinsBuild>
    {
        private readonly ILogger _logger;
        private readonly IDialogService _dialogService;

        public TriggerJenkinsBuild(IDialogService dialogService, ILogger logger)
        {
            _logger = Preconditions.CheckNotNull(logger, nameof(logger));
            _dialogService = Preconditions.CheckNotNull(dialogService, nameof(dialogService));
        }

        public async Task HandleAsync(JenkinsBuild build)
        {
            var result = _dialogService.Show($"Are you sure you want to trigger {build.Name}?", "Trigger a new build", MessageBoxButton.YesNo, MessageBoxImage.Asterisk);

            if (result != MessageBoxResult.Yes)
                return;

            await QueueNewBuild(build);

            _logger.Info($"Build \"{build.Name}\" was triggered.");
        }

        private async Task QueueNewBuild(JenkinsBuild build)
        {
            using (var handler = new WebRequestHandler())
            {
                handler.UseDefaultCredentials = true;

                if (build.IgnoreSslErrors)
                {
                    handler.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                }

                using (var client = new HttpClient(handler))
                {
                    ConfigureHttpClientAuthorization(build, client);

                    var baseUri = new Uri(build.Url);

                    var uri = new Uri(baseUri, "buildWithParameters?delay=0sec");

                    var response = await client.PostAsync(uri, new StringContent(string.Empty));

                    response.EnsureSuccessStatusCode();
                }
            }
        }

        private static void ConfigureHttpClientAuthorization(JenkinsBuild item, HttpClient client)
        {
            if (string.IsNullOrEmpty(item.UserName) || string.IsNullOrEmpty(item.ApiToken))
                return;

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Basic", Convert.ToBase64String(
                        Encoding.ASCII.GetBytes(string.Format("{0}:{1}", item.UserName, item.ApiToken))));
        }
    }

    public class JenkinsBuildMonitor : IMonitor<JenkinsBuild>
    {
        [DebuggerStepThrough]
        public void Handle(JenkinsBuild item)
        {
            var build = GetBuildDetailsAsync(item).Result;

            if (build.Building)
            {
                item.State = State.Running;
                return;
            }

            switch (build.Result)
            {
                case "SUCCESS":
                    item.State = State.Ok;
                    break;
                case "ABORTED":
                    item.State = State.Canceled;
                    break;
                case "FAILURE":
                    item.State = State.Failed;
                    break;
                case "UNSTABLE":
                    item.State = State.PartiallySucceeded;
                    break;
                default:
                    item.State = State.Unknown;
                    break;
            }
        }

        private async Task<JenkinsBuildDetails> GetBuildDetailsAsync(JenkinsBuild item)
        {
            using (var handler = new WebRequestHandler())
            {
                handler.UseDefaultCredentials = true;

                if (item.IgnoreSslErrors)
                {
                    handler.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                }

                using (var client = new HttpClient(handler))
                {
                    ConfigureHttpClientAuthorization(item, client);

                    var baseUri = new Uri(item.Url);

                    var apiUrl = new Uri(baseUri, "lastBuild/api/json?tree=result,building");

                    var response = await client.GetAsync(apiUrl);

                    response.EnsureSuccessStatusCode();

                    var content = await response.Content.ReadAsStringAsync();

                    var buildDetails = new JavaScriptSerializer().Deserialize<JenkinsBuildDetails>(content);

                    if (buildDetails == null)
                        throw new Exception("Invalid Jenkins Build response.");

                    return buildDetails;
                }
            }
        }

        private static void ConfigureHttpClientAuthorization(JenkinsBuild item, HttpClient client)
        {
            if (string.IsNullOrEmpty(item.UserName) || string.IsNullOrEmpty(item.ApiToken))
                return;

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Basic", Convert.ToBase64String(
                        Encoding.ASCII.GetBytes(string.Format("{0}:{1}", item.UserName, item.ApiToken))));
        }
    }

    #region Contracts

    public class JenkinsBuildDetails
    {
        public bool Building { get; set; }

        public string Result { get; set; }
    }

    #endregion
}