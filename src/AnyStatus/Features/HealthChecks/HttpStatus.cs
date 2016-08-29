using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Windows.Media;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus.Models
{
    [DisplayName("HTTP Status")]
    [Description("")]
    public class HttpStatus : Item
    {
        public HttpStatus()
        {
            HttpStatusCode = HttpStatusCode.OK;
        }

        [PropertyOrder(1)]
        public string Url { get; set; }

        [PropertyOrder(2)]
        [DisplayName("HTTP Status Code")]
        public HttpStatusCode HttpStatusCode { get; set; }

        [PropertyOrder(3)]
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
                {
                    handler.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                }

                using (var client = new HttpClient(handler))
                {
                    var response = client.GetAsync(item.Url).Result;

                    item.Brush = response.StatusCode == item.HttpStatusCode ? Brushes.Green : Brushes.Red;
                }
            }
        }
    }
}
