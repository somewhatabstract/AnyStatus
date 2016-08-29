﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus.Models
{
    [DisplayName("TFS 2015 Build")]
    [Description("Microsoft Team Foundation Server 2015 build status.")]
    public class TfsBuild : Item
    {
        public TfsBuild()
        {
            Collection = "DefaultCollection";
        }

        [PropertyOrder(1)]
        [Description("Visual Studio Team Services account (https://{account}.visualstudio.com) or TFS server (http://{server:port}/tfs).")]
        public string Url { get; set; }

        [PropertyOrder(2)]
        [Description()]
        public string Collection { get; set; }

        [PropertyOrder(3)]
        [DisplayName("Team Project")]
        [Description()]
        public string TeamProject { get; set; }

        [PropertyOrder(4)]
        [DisplayName("Build Definition")]
        [Description()]
        public string BuildDefinition { get; set; }

        [PropertyOrder(5)]
        [DisplayName("User Name")]
        public string UserName { get; set; }

        [PropertyOrder(6)]
        [PasswordPropertyText(true)]
        public string Password { get; set; }
    }

    public class TfsBuildHandler : IHandler<TfsBuild>
    {
        public void Handle(TfsBuild item)
        {
            Validate(item);

            var buildDefinitionId = GetBuildDefinitionIdAsync(item).Result;

            //var build = GetBuildDetailsAsync(item).Result;
        }

        private async Task<int> GetBuildDefinitionIdAsync(TfsBuild item)
        {
            var useDefaultCredentials = string.IsNullOrEmpty(item.UserName) && string.IsNullOrEmpty(item.Password);

            using (var handler = new WebRequestHandler())
            {
                handler.UseDefaultCredentials = useDefaultCredentials;

                using (var client = new HttpClient(handler))
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    if (!useDefaultCredentials)
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

        private async Task<TfsBuildDetails> GetBuildDetailsAsync(TfsBuild item)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json-patch+json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", item.UserName, item.Password))));

                var apiUrl = $"{item.Url}/{item.Collection}/{item.TeamProject}/_apis/build/builds?definitions={item.BuildDefinition}&statusFilter=completed&$top=1&api-version=2.0";

                var response = await client.GetAsync(apiUrl);

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();

                var buildResponse = new JavaScriptSerializer().Deserialize<TfsBuildDetails>(content);

                return null;
            }
        }

        private void Validate(TfsBuild item)
        {
            if (item == null || item.Url == null)
            {
                throw new InvalidOperationException("Invalid item.");
            }
        }
    }

    public class BuildDefinitionResponse
    {
        public List<BuildDefinitionDetails> Value { get; set; }
    }

    public class BuildDefinitionDetails
    {
        public int Id { get; set; }
    }

    public class TfsBuildDetails
    {
    }
}