using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Script.Serialization;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus
{
    [DisplayName("Coveralls")]
    [Description("Shows the covered code percentage")]
    public class CoverallsCoveredPercent : Metric, IScheduledItem
    {
        private const string Category = "Coveralls";

        public CoverallsCoveredPercent()
        {
            Threshold = 80;
        }

        [Url]
        [Required]
        [PropertyOrder(10)]
        [Category(Category)]
        [Description("Coveralls repository URL address. For example: https://coveralls.io/github/AlonAm/AnyStatus")]
        public string Url { get; set; }

        [Required]
        [PropertyOrder(20)]
        [Category(Category)]
        [Description("")]
        public int Threshold { get; set; }
    }

    public class CoverallsMonitor : IMonitor<CoverallsCoveredPercent>
    {
        [DebuggerStepThrough]
        public void Handle(CoverallsCoveredPercent item)
        {
            var uri = new Uri(item.Url);

            var endpoint = uri.GetLeftPart(UriPartial.Path) + ".json" + uri.Query;

            using (var httpClient = new HttpClient())
            {
                var httpResponse = httpClient.GetAsync(endpoint).Result;

                httpResponse.EnsureSuccessStatusCode();

                var content = httpResponse.Content.ReadAsStringAsync().Result;

                var response = new JavaScriptSerializer()
                        .Deserialize<CoveredPercentResponse>(content);

                if (response == null)
                {
                    item.State = State.Error;
                    item.Value = string.Empty;
                    return;
                }

                item.Value = response.CoveredPercent + "%";

                item.State = response.CoveredPercent < item.Threshold ? State.Failed : State.Ok;
            }
        }

        #region Contracts

        class CoveredPercentResponse
        {
            internal float covered_percent { private get; set; }

            public int CoveredPercent
            {
                get
                {
                    return (int)covered_percent;
                }
            }
        }

        #endregion
    }
}
