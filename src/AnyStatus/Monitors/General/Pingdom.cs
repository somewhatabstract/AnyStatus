using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Script.Serialization;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus
{
    [DisplayName("Pingdom")]
    [DisplayColumn("General")]
    public class Pingdom : Item, IScheduledItem
    {
        private const string Category = "Pingdom";

        [Required]
        [PropertyOrder(10)]
        [Category(Category)]
        [DisplayName("User Name")]
        [Description("Required. Pingdom account user name (Email).")]
        public string UserName { get; set; }

        [Required]
        [PropertyOrder(20)]
        [Category(Category)]
        [DisplayName("Password")]
        [Description("Required. Pingdom account password.")]
        [Editor(typeof(PasswordEditor), typeof(PasswordEditor))]
        public string Password { get; set; }

        [PropertyOrder(30)]
        [Category(Category)]
        [DisplayName("Check Id")]
        [Description("Optional. Leave empty for the overall status of all checks.")]
        public string CheckId { get; set; }
    }

    public class PingdomMonitor : IMonitor<Pingdom>
    {
        private const string ApiKey = "mrrbba50jz2zjmrar2uqbj5rovtijq1b";
        private const string ServerAddress = "https://api.pingdom.com/api/2.0/checks/";

        [DebuggerStepThrough]
        public void Handle(Pingdom pingdom)
        {
            PingdomCheckStatus status;
            
            if (string.IsNullOrWhiteSpace(pingdom.CheckId))
            {
                var checkList = GetChecks<CheckList>(ServerAddress,pingdom.UserName, pingdom.Password);

                status = checkList.Checks.Max(k=>k.Status);
            }
            else
            {
                var check = GetChecks<Check>(ServerAddress + pingdom.CheckId, pingdom.UserName, pingdom.Password);

                status = check.Status;
            }

            pingdom.State = GetState(status);
        }

        private State GetState(PingdomCheckStatus status)
        {
            switch (status)
            {
                case PingdomCheckStatus.Up:
                    return State.Ok;
                case PingdomCheckStatus.Paused:
                    //todo: add Paused state
                    return State.Canceled;
                case PingdomCheckStatus.Unknown:
                    return State.Unknown;
                case PingdomCheckStatus.Unconfirmed_down:
                case PingdomCheckStatus.Down:
                    return State.Failed;
                default:
                    return State.Unknown;
            }
        }

        private T GetChecks<T>(string endpoint, string username, string password)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                            Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}")));

                client.DefaultRequestHeaders.Add("App-Key", ApiKey);

                var httpResponse = client.GetAsync(ServerAddress).Result;

                httpResponse.EnsureSuccessStatusCode();

                var content = httpResponse.Content.ReadAsStringAsync().Result;

                return new JavaScriptSerializer().Deserialize<T>(content);
            }
        }

        #region Contracts

        public enum PingdomCheckStatus
        {
            Up,
            Paused,
            Unknown,
            Unconfirmed_down,
            Down,
        }

        public class CheckList
        {
            public IEnumerable<Check> Checks { get; set; }
        }

        public class Check
        {
            public PingdomCheckStatus Status { get; set; }
        }

        #endregion
    }
}
