using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus
{
    [DisplayName("HTTP")]
    [Description("")]
    public class HttpStatus : Item, IScheduledItem
    {
        public HttpStatus()
        {
            HttpStatusCode = HttpStatusCode.OK;
        }

        [Required]
        [Category("HTTP")]
        [PropertyOrder(10)]
        [DisplayName("URL")]
        public string Url { get; set; }

        [PropertyOrder(20)]
        [Category("HTTP")]
        [DisplayName("HTTP Status Code")]
        public HttpStatusCode HttpStatusCode { get; set; }

        [PropertyOrder(30)]
        [DisplayName("Ignore SSL Errors")]
        public bool IgnoreSslErrors { get; set; }
    }

    public class HttpStatusHandler : IHandler<HttpStatus>
    {
        [DebuggerStepThrough]
        public void Handle(HttpStatus item)
        {
            using (var handler = new WebRequestHandler())
            {
                if (item.IgnoreSslErrors)
                    handler.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

                try
                {
                    using (var client = new HttpClient(handler))
                    {
                        var response = client.GetAsync(item.Url).Result;

                        item.State = response.StatusCode == item.HttpStatusCode ? State.Ok : State.Failed;
                    }
                }
                catch (AggregateException ae)
                {
                    ae.Handle(ex =>
                    {
                        if (ex is HttpRequestException)
                            item.State = State.Failed;

                        return ex is HttpRequestException;
                    });
                }
            }
        }
    }
}