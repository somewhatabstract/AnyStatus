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
using System.Xml.Serialization;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus
{
    [DisplayName("TFS 2015 Build")]
    [Description("Microsoft Team Foundation Server 2015 or Visual Studio Team Services build status")]
    public class TfsBuild : Item, IScheduledItem, ICanOpenInBrowser, ICanTriggerBuild
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

    public abstract class BaseTfsBuildHandler
    {
        public virtual void Handle(TfsBuild item)
        {
            if (item.BuildDefinitionId <= 0)
            {
                item.BuildDefinitionId = GetBuildDefinitionIdAsync(item).Result;
            }
        }

        protected async Task<int> GetBuildDefinitionIdAsync(TfsBuild item)
        {
            using (var handler = new WebRequestHandler())
            {
                handler.UseDefaultCredentials = string.IsNullOrEmpty(item.UserName) || string.IsNullOrEmpty(item.Password); ;

                using (var client = new HttpClient(handler))
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    if (handler.UseDefaultCredentials == false)
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
    }

    public class TfsBuildMonitor : BaseTfsBuildHandler, IMonitor<TfsBuild>
    {
        [DebuggerStepThrough]
        public override void Handle(TfsBuild item)
        {
            base.Handle(item);

            var buildDetails = GetBuildDetailsAsync(item).Result;

            if (buildDetails.Status == "notStarted" || buildDetails.Status == "inProgress")
            {
                item.State = State.Running;
                return;
            }

            switch (buildDetails.Result)
            {
                case "notStarted":
                    item.State = State.Running;
                    break;
                case "succeeded":
                    item.State = State.Ok;
                    break;
                case "failed":
                    item.State = State.Failed;
                    break;
                case "partiallySucceeded":
                    item.State = State.PartiallySucceeded;
                    break;
                case "canceled":
                    item.State = State.Canceled;
                    break;
                default:
                    item.State = State.Unknown;
                    break;
            }
        }

        private async Task<TfsBuildDetails> GetBuildDetailsAsync(TfsBuild item)
        {
            using (var handler = new WebRequestHandler())
            {
                handler.UseDefaultCredentials = string.IsNullOrEmpty(item.UserName) || string.IsNullOrEmpty(item.Password); ;

                using (var client = new HttpClient(handler))
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    if (handler.UseDefaultCredentials == false)
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
    }

    public class TriggerTfsBuild : BaseTfsBuildHandler, ITriggerBuild<TfsBuild>
    {
        [DebuggerStepThrough]
        public override void Handle(TfsBuild item)
        {
            if (item.IsValid() == false)
                return;

            base.Handle(item);

            QueueNewBuild(item);
        }

        private void QueueNewBuild(TfsBuild item)
        {
            using (var handler = new WebRequestHandler())
            {
                handler.UseDefaultCredentials = string.IsNullOrEmpty(item.UserName) || string.IsNullOrEmpty(item.Password); ;

                using (var client = new HttpClient(handler))
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    if (handler.UseDefaultCredentials == false)
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                            Convert.ToBase64String(Encoding.ASCII.GetBytes($"{item.UserName}:{item.Password}")));
                    }

                    var url = $"{item.Url}/{item.Collection}/{item.TeamProject}/_apis/build/builds?api-version=2.0";

                    var request = new QueueNewBuildRequest
                    {
                        Definition = new Definition
                        {
                            Id = item.BuildDefinitionId
                        }
                    };

                    var json = new JavaScriptSerializer().Serialize(request);

                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = client.PostAsync(url, content).Result;

                    response.EnsureSuccessStatusCode();
                }
            }
        }
    }

    public class OpenTfsBuildInBrowser : BaseTfsBuildHandler, IOpenInBrowser<TfsBuild>
    {
        private readonly ILogger _logger;
        private readonly IProcessStarter _processStarter;

        public OpenTfsBuildInBrowser(IProcessStarter processStarter, ILogger logger)
        {
            _logger = Preconditions.CheckNotNull(logger, nameof(logger));
            _processStarter = Preconditions.CheckNotNull(processStarter, nameof(processStarter));
        }

        public override void Handle(TfsBuild item)
        {
            if (item.IsValid() == false)
                return;

            base.Handle(item);

            var uri = $"{item.Url}/{item.Collection}/{item.TeamProject}/_build?_a=completed&definitionId={item.BuildDefinitionId}";

            _processStarter.Start(uri.ToString());
        }
    }

    #region Contracts

    internal class QueueNewBuildRequest
    {
        public Definition Definition { get; set; }
    }

    internal class Definition
    {
        public int Id { get; set; }
    }

    internal class BuildDefinitionResponse
    {
        public List<BuildDefinitionDetails> Value { get; set; }
    }

    internal class BuildDefinitionDetails
    {
        public int Id { get; set; }
    }

    internal class TfsBuildDetailsResponse
    {
        public List<TfsBuildDetails> Value { get; set; }
    }

    internal class TfsBuildDetails
    {
        public string Result { get; set; }

        public string Status { get; set; }
    }

    #endregion
}
