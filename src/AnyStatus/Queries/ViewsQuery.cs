using AnyStatus.Models;
using RestSharp;
using RestSharp.Serializers;
using System.Collections.Generic;

namespace AnyStatus.Queries
{
    public class ViewsQuery
    {
        public class Request
        {
            public Server Server { get; set; }
        }

        public class Response
        {
            [SerializeAs(Name = "views")]
            public List<View> Views { get; set; }
        }

        public class Handler
        {
            public Response Handle(Request request)
            {
                var client = new RestClient(request.Server.Url);
                
                var restRequest = new RestRequest("api/json?tree=views[name,url]");

                var response = client.Execute<Response>(restRequest);

                return response.Data;
            }
        }
    }
}
