namespace AnyStatus
{
    public interface IUsageReporter
    {
        bool IsEnabled { get; set; }

        string ClientId { get; set; }

        void ReportEvent(string category, string action, string label, int? value = null);

        void ReportScreen(string name);

        void ReportStartSession();

        void ReportEndSession();
    }
}