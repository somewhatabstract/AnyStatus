using System.ComponentModel;

namespace AnyStatus.Models
{
    [DisplayName("Jenkins Job")]//todo: wire display-name to template name
    public class JenkinsJob : Item
    {
        public string Url { get; set; }

        public string UserName { get; set; }

        public string ApiToken { get; set; }
    }

    public class JenkinsJobHandler : IHandler<JenkinsJob>
    {
        public void Handle(JenkinsJob job)
        {
        }
    }
}