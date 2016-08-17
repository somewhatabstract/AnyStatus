using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Windows.Media;

namespace AnyStatus.Models
{
    [DisplayName("HTTP Status")]
    public class HttpStatus : Item
    {
        public HttpStatus()
        {
            HttpStatusCode = HttpStatusCode.OK;
        }

        public string Url { get; set; }

        [DisplayName("HTTP Status Code")]
        public HttpStatusCode HttpStatusCode { get; set; }
    }

    public class HttpStatusHandler : IHandler<HttpStatus>
    {
        public void Handle(HttpStatus item)
        {
            using (var client = new HttpClient())
            {
                var response = client.GetAsync(item.Url).Result;

                item.Brush = response.StatusCode == item.HttpStatusCode ? Brushes.Green : Brushes.Red;
            }
        }
    }
}
