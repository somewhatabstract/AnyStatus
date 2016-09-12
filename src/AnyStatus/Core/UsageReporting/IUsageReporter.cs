namespace AnyStatus
{
    public interface IUsageReporter
    {
        void ReportEvent(string category, string action, string label, int? value = null);

        void ReportScreen(string name);

        void ReportStartSession();

        void ReportEndSession();
    }
}