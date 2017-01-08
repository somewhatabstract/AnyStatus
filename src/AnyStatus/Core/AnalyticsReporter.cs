using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace AnyStatus
{
    /// <summary>
    /// <para>
    /// Client for the Google Analytics Measurement Protocol service, which makes
    /// HTTP requests to publish data to a Google Analytics account
    /// </para>
    /// 
    /// <para>
    /// For more information, see:
    /// https://developers.google.com/analytics/devguides/collection/protocol/v1/
    /// </para>
    /// </summary>
    public class AnalyticsReporter : IUsageReporter
    {
        #region Fields
        private static TimeSpan Timeout = TimeSpan.FromSeconds(3);

        private const string ProductionServerUrl = "https://www.google-analytics.com/collect";
        private const string DebugServerUrl = "https://www.google-analytics.com/debug/collect";

        private const string HitTypeParam = "t";
        private const string VersionParam = "v";
        private const string EventCategoryParam = "ec";
        private const string EventActionParam = "ea";
        private const string EventLabelParam = "el";
        private const string EventValueParam = "ev";
        private const string SessionControlParam = "sc";
        private const string PropertyIdParam = "tid";
        private const string ClientIdParam = "cid";
        private const string AppNameParam = "an";
        private const string AppIdParam = "aid";
        private const string AppVersionParam = "av";
        private const string ScreenNameParam = "cd";

        private const string VersionValue = "1";
        private const string EventTypeValue = "event";
        private const string SessionStartValue = "start";
        private const string SessionEndValue = "end";
        private const string ScreenViewValue = "screenview";

        private readonly string _serverUrl;
        private readonly Dictionary<string, string> _baseHitData;

        #endregion

        public AnalyticsReporter()
        {
            PropertyId = "UA-83802855-1";
            ApplicationId = "AnyStatus";
            ApplicationName = "AnyStatus";
            ApplicationVersion = "0.11";

            _serverUrl = ProductionServerUrl;
            _baseHitData = MakeBaseHitData();
        }

        #region Properties

        public bool IsEnabled { get; set; }

        public string ApplicationName { get; }

        public string ApplicationVersion { get; }

        public string ApplicationId { get; }

        public string PropertyId { get; }

        public string ClientId { get; set; }

        #endregion

        #region Public Methods

        public void ReportEvent(string category, string action, string label, int? value = null)
        {
            if (!IsEnabled) return;

            Preconditions.CheckNotNull(category, nameof(category));
            Preconditions.CheckNotNull(action, nameof(action));

            var hitData = new Dictionary<string, string>(_baseHitData)
            {
                { ClientIdParam, ClientId },
                { HitTypeParam, EventTypeValue },
                { EventCategoryParam, category },
                { EventActionParam, action },
                { EventLabelParam, label },
            };

            if (value != null)
            {
                hitData[EventValueParam] = value.ToString();
            }

            SendHitDataAsync(hitData);
        }

        public void ReportScreen(string name)
        {
            if (!IsEnabled) return;

            Preconditions.CheckNotNull(name, nameof(name));

            var hitData = new Dictionary<string, string>(_baseHitData)
            {
                { ClientIdParam, ClientId },
                { HitTypeParam, ScreenViewValue },
                { ScreenNameParam, name },
            };

            SendHitDataAsync(hitData);
        }

        public void ReportStartSession()
        {
            if (!IsEnabled) return;

            var hitData = new Dictionary<string, string>(_baseHitData)
            {
                { ClientIdParam, ClientId },
                { HitTypeParam, EventTypeValue },
                { SessionControlParam, SessionStartValue },
                { EventCategoryParam, "Session" },
                { EventActionParam, "Start" },
                { EventLabelParam, "Start Session" },
            };

            SendHitDataAsync(hitData);
        }

        public void ReportEndSession()
        {
            if (!IsEnabled) return;

            var hitData = new Dictionary<string, string>(_baseHitData)
            {
                { ClientIdParam, ClientId },
                { HitTypeParam, EventTypeValue },
                { SessionControlParam, SessionEndValue },
                { EventCategoryParam, "Session" },
                { EventActionParam, "End" },
                { EventLabelParam, "End Session" },
            };

            SendHitData(hitData);
        }

        #endregion

        #region Helpers

        private Dictionary<string, string> MakeBaseHitData()
        {
            var result = new Dictionary<string, string>
            {
                { VersionParam, VersionValue },
                { PropertyIdParam, PropertyId },
                { AppNameParam, ApplicationName },
                { AppIdParam, ApplicationId },
                { AppVersionParam, ApplicationVersion },
            };

            return result;
        }

        private void SendHitDataAsync(Dictionary<string, string> hitData)
        {
            try
            {
                Task.Run(async () =>
                {
                    using (var client = new HttpClient())
                    using (var form = new FormUrlEncodedContent(hitData))
                        await client.PostAsync(_serverUrl, form).ConfigureAwait(false);
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private void SendHitData(Dictionary<string, string> hitData)
        {
            try
            {
                using (var client = new HttpClient())
                using (var form = new FormUrlEncodedContent(hitData))
                {
                    client.Timeout = Timeout;
                    client.PostAsync(_serverUrl, form).Wait();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        [Conditional("DEBUG")]
        private async void DebugPrintAnalyticsOutput(Task<string> resultTask)
        {
            var result = await resultTask.ConfigureAwait(false);

            Debug.WriteLine($"Output of analytics: {result}");
        }

        #endregion
    }
}
