using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus
{
    [DisplayName("Jenkins Build")]
    public class JenkinsBuild : Item, IScheduledItem, ICanOpenInBrowser
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
            if (item == null || string.IsNullOrEmpty(item.Url))
                return;

            try
            {
                _processStarter.Start(item.Url);
            }
            catch
            {
            }
        }
    }

    public class JenkinsBuildHandler : IHandler<JenkinsBuild>
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

                    var apiUrl = $"{item.Url}/lastBuild/api/json?tree=result,building";

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

    public class JenkinsBuildDetails
    {
        public bool Building { get; set; }

        public string Result { get; set; }
    }
}