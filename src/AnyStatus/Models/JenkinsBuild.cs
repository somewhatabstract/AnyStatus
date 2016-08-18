using RestSharp;
using RestSharp.Serializers;
using System;
using System.ComponentModel;
using System.Windows.Media;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus.Models
{
    [DisplayName("Jenkins Build")] //todo: wire display-name to template name
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

        //[PropertyOrder(3)]
        //[DisplayName("Ignore SSL Errors")]
        //public bool IgnoreSslErrors { get; set; }
    }

    public class JenkinsBuildHandler : IHandler<JenkinsBuild>
    {
        private const string ApiUrl = "/lastBuild/api/json?tree=result,building";

        public void Handle(JenkinsBuild item)
        {
            Validate(item);

            var build = GetBuildDetails(item);

            if (build.IsInProgress)
            {
                build.Result = "INPROGRESS";
            }

            SetItemColor(item, build.Result);
        }

        private static void Validate(JenkinsBuild item)
        {
            //todo: replace with validation decorator

            if (item == null || string.IsNullOrEmpty(item.Url))
            {
                throw new InvalidOperationException("Invalid item."); 
            }
        }

        private static JenkinsBuildResponse GetBuildDetails(JenkinsBuild item)
        {
            var client = new RestClient(item.Url);

            var response = client.Execute<JenkinsBuildResponse>(new RestRequest(ApiUrl));

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception($"Unexpected HTTP status code {response.StatusCode}");
            }

            return response.Data;
        }

        private static void SetItemColor(JenkinsBuild item, string status)
        {
            switch (status)
            {
                case "INPROGRESS":
                    item.Brush = Brushes.DodgerBlue;
                    break;

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

    public class JenkinsBuildResponse
    {
        public bool IsInProgress
        {
            get
            {
                return bool.Parse(Building);
            }
        }

        [SerializeAs(Name = "building")]
        public string Building { get; set; }

        [SerializeAs(Name = "result")]
        public string Result { get; set; }
    }
}