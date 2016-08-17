﻿using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Windows.Media;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus.Models
{
    [DisplayName("HTTP Status")]
    public class HttpStatus : Item
    {
        public HttpStatus()
        {
            HttpStatusCode = HttpStatusCode.OK;
        }

        [PropertyOrder(0)]
        public string Url { get; set; }

        [PropertyOrder(1)]
        [DisplayName("HTTP Status Code")]
        public HttpStatusCode HttpStatusCode { get; set; }

        [PropertyOrder(2)]
        [DisplayName("Ignore SSL Errors")]
        public bool IgnoreSslErrors { get; set; }
    }

    public class HttpStatusHandler : IHandler<HttpStatus>
    {
        public void Handle(HttpStatus item)
        {
            using (var handler = new WebRequestHandler())
            {
                if (item.IgnoreSslErrors)
                {
                    handler.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                }

                using (var client = new HttpClient())
                {
                    var response = client.GetAsync(item.Url).Result;

                    item.Brush = response.StatusCode == item.HttpStatusCode ? Brushes.Green : Brushes.Red;
                }
            }
        }
    }
}
