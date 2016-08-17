using RestSharp;
using RestSharp.Serializers;
using System.ComponentModel;
using System.Windows.Media;

namespace AnyStatus.Models
{
    [DisplayName("Jenkins Build")] //todo: wire display-name to template name
    public class JenkinsBuild : Item
    {
        public string Url { get; set; }

        [DisplayName("User Name")]
        public string UserName { get; set; }

        [DisplayName("API Token")]
        public string ApiToken { get; set; }
    }

    public class JenkinsJobHandler : IHandler<JenkinsBuild>
    {
        public void Handle(JenkinsBuild item)
        {
            if (item == null || string.IsNullOrEmpty(item.Url))
            {
                return;
            }

            var client = new RestClient(item.Url);

            var restRequest = new RestRequest("/lastBuild/api/json?tree=result[*]");

            var response = client.Execute<JenkinsBuildResponse>(restRequest);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                switch (response.Data.Result)
                {
                    case "SUCCESS":
                        item.Brush = Brushes.Green;
                        break;

                    case "ABORTED":
                        item.Brush = Brushes.Green;
                        break;

                    case "FAILURE":
                        item.Brush = Brushes.Red;
                        break;

                    case "UNSTABLE":
                        item.Brush = Brushes.Yellow;
                        break;

                    default:
                        break;
                }
            }
        }
    }

    public class JenkinsBuildResponse
    {
        [SerializeAs(Name = "result")]
        public string Result { get; set; }
    }
}