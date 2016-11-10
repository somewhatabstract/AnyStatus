using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Media;

namespace AnyStatus.Models
{
    [DisplayName("GitHub Issue")]
    [Description("")]
    public class GitHubIssue : Item, IScheduledItem
    {
        [Required]
        [Category("GitHub")]
        [Description("The GitHub repository owner")]
        public string Owner { get; set; }

        [Required]
        [Category("GitHub")]
        [Description("The GitHub repository name")]
        public string Repository { get; set; }

        [Required]
        [Category("GitHub")]
        [DisplayName("Issue Number")]
        [Description("The GitHub issue number")]
        public int IssueNumber { get; set; }
    }

    public class GitHubIssueHandler : IHandler<GitHubIssue>
    {
        public void Handle(GitHubIssue item)
        {
            var state = GetGitHubIssueStateAsync(item).Result;

            switch (state)
            {
                case GitHubIssueState.Open:
                    item.State = State.Open;
                    break;
                case GitHubIssueState.Closed:
                    item.State = State.Closed;
                    break;
                default:
                    item.State = State.Unknown;
                    break;
            }
        }

        private async Task<GitHubIssueState> GetGitHubIssueStateAsync(GitHubIssue item)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("ANYSTATUS", "1.0"));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var apiUrl = $"https://api.github.com/repos/{item.Owner}/{item.Repository}/issues/{item.IssueNumber}";

                var response = await client.GetAsync(apiUrl);

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();

                var githubIssue = new JavaScriptSerializer().Deserialize<GitHubIssueDetails>(content);

                return githubIssue.State;
            }
        }

        private class GitHubIssueDetails
        {
            public GitHubIssueState State { get; set; }
        }
    }

    public enum GitHubIssueState
    {
        None,
        Open,
        Closed,
        All
    }
}
