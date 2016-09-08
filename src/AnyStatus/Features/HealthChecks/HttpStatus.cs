using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Windows.Media;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus.Models
{
    [DisplayName("HTTP Status")]
    [Description("")]
    public class HttpStatus : Item, IScheduledItem
    {
        public HttpStatus()
        {
            HttpStatusCode = HttpStatusCode.OK;
        }

        [Required]
        [Category("HTTP Status")]
        [PropertyOrder(10)]
        public string Url { get; set; }

        [PropertyOrder(20)]
        [Category("HTTP Status")]
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
