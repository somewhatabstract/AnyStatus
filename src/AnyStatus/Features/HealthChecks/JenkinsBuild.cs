using System;
using System.ComponentModel;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Media;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus.Models
{
    [DisplayName("Jenkins Build")]
    public class JenkinsBuild : Item
    {
        [PropertyOrder(1)]
        [Description("Jenkins build URL address.")]
        public string Url { get; set; }

        //[PropertyOrder(1)]
        //[DisplayName("User Name")]
        //public string UserName { get; set; }

        //[PropertyOrder(2)]
        //[DisplayName("API Token")]
        //public string ApiToken { get; set; }

        [PropertyOrder(3)]
        [DisplayName("Ignore SSL Errors")]
        public bool IgnoreSslErrors { get; set; }
    }

    public class JenkinsBuildHandler : IHandler<JenkinsBuild>
    {
        public void Handle(JenkinsBuild item)
        {
            Validate(item);//todo: move to handler validation decorator

            var build = GetBuildDetailsAsync(item).Result;

            SetItemColor(item, build);
        }

        private async Task<JenkinsBuildDetails> GetBuildDetailsAsync(JenkinsBuild item)
        {
            using (var handler = new WebRequestHandler())
            {
                if (item.IgnoreSslErrors)
                {
                    handler.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                }

                using (var client = new HttpClient(handler))
                {
                    var apiUrl = $"{item.Url}/lastBuild/api/json?tree=result,building";

                    var response = await client.GetAsync(apiUrl);

                    response.EnsureSuccessStatusCode();

                    var content = await response.Content.ReadAsStringAsync();

                    var buildDetails = new JavaScriptSerializer().Deserialize<JenkinsBuildDetails>(content);

                    return buildDetails;
                }
            }
        }

        private static void Validate(JenkinsBuild item)
        {
            if (item == null || string.IsNullOrEmpty(item.Url))
            {
                throw new InvalidOperationException("Invalid item.");
            }
        }

        private static void SetItemColor(JenkinsBuild item, JenkinsBuildDetails build)
        {
            if (build.IsInProgress)
            {
                item.Brush = Brushes.DodgerBlue;
                return;
            }

            switch (build.Result)
            {
                case "SUCCESS":
                    item.Brush = Brushes.Green;
                    break;

                case "ABORTED":
                    item.Brush = Brushes.Gray;
                    break;

                case "FAILURE":
                    item.Brush = Brushes.Red;
                    break;

                case "UNSTABLE":
                    item.Brush = Brushes.Orange;
                    break;

                default:
                    break;
            }
        }
    }

    public class JenkinsBuildDetails
    {
        public bool IsInProgress
        {
            get
            {
                return Building;
            }
        }

        public bool Building { get; set; }

        public string Result { get; set; }
    }
}