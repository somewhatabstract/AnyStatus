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
        public void Handle(JenkinsBuild job)
        {
            if (job == null || string.IsNullOrEmpty(job.Url))
            {
                return;
            }

            var client = new RestClient(job.Url);

            var restRequest = new RestRequest("/lastBuild/api/json");

            var response = client.Execute<JenkinsBuildResponse>(restRequest);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                switch (response.Data.Result)
                {
                    case "SUCCESS":
                        job.Brush = Brushes.Green;
                        break;

                    case "ABORTED":
                        job.Brush = Brushes.Green;
                        break;

                    case "FAILURE":
                        job.Brush = Brushes.Red;
                        break;

                    case "UNSTABLE":
                        job.Brush = Brushes.Yellow;
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