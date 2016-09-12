using AnyStatus.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public class AnalyticsReporter: IUsageReporter
    {
        #region Fields

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

        private readonly bool _debug;
        private readonly string _serverUrl;
        private readonly Dictionary<string, string> _baseHitData;

        #endregion

        #region Properties

        public string ApplicationName { get; }

        public string ApplicationVersion { get; }

        public string ApplicationId { get; }

        public string PropertyId { get; }

        public string ClientId { get; }

        public bool IsEnabled { get; set; }

        #endregion

        public AnalyticsReporter(
            string propertyId,
            string appName,
            string appId,
            string clientId = null,
            string appVersion = null,
            bool debug = false)
        {
            PropertyId = Preconditions.CheckNotNull(propertyId, nameof(propertyId));
            ApplicationName = Preconditions.CheckNotNull(appName, nameof(appName));
            ApplicationId = Preconditions.CheckNotNull(appId, nameof(appId));
            ClientId = clientId ?? Guid.NewGuid().ToString();
            ApplicationVersion = appVersion;

            _debug = debug;
            _serverUrl = debug ? DebugServerUrl : ProductionServerUrl;
            _baseHitData = MakeBaseHitData();
        }

        #region Public Methods

        public void ReportEvent(string category, string action, string label, int? value = null)
        {
            if (!IsEnabled) return;

            Preconditions.CheckNotNull(category, nameof(category));
            Preconditions.CheckNotNull(action, nameof(action));

            var hitData = new Dictionary<string, string>(_baseHitData)
            {
                { HitTypeParam, EventTypeValue },
                { EventCategoryParam, category },
                { EventActionParam, action },
                { EventLabelParam, label },
            };

            if (value != null)
            {
                hitData[EventValueParam] = value.ToString();
            }

            SendHitData(hitData);
        }

        public void ReportScreen(string name)
        {
            if (!IsEnabled) return;

            Preconditions.CheckNotNull(name, nameof(name));

            var hitData = new Dictionary<string, string>(_baseHitData)
            {
                { HitTypeParam, ScreenViewValue },
                { ScreenNameParam, name },
            };

            SendHitData(hitData);
        }

        public void ReportStartSession()
        {
            if (!IsEnabled) return;

            var hitData = new Dictionary<string, string>(_baseHitData)
            {
                { HitTypeParam, EventTypeValue },
                { SessionControlParam, SessionStartValue },
                { EventCategoryParam, "Session" },
                { EventActionParam, "Start" },
                { EventLabelParam, "Start Session" },
            };

            SendHitData(hitData);
        }

        public void ReportEndSession()
        {
            if (!IsEnabled) return;

            var hitData = new Dictionary<string, string>(_baseHitData)
            {
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
                { ClientIdParam, ClientId },
                { AppNameParam, ApplicationName },
                { AppIdParam, ApplicationId },
            };

            if (ApplicationVersion != null)
            {
                result.Add(AppVersionParam, ApplicationVersion);
            }

            return result;
        }

        private async void SendHitData(Dictionary<string, string> hitData)
        {
            if (!IsEnabled) return;

            try
            {
                using (var client = new HttpClient())
                using (var form = new FormUrlEncodedContent(hitData))
                using (var response = await client.PostAsync(_serverUrl, form).ConfigureAwait(false))
                {
                    DebugPrintAnalyticsOutput(response.Content.ReadAsStringAsync());
                }
            }
            catch
            {
                // Ignore
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
