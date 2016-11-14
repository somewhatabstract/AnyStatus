using System;

namespace AnyStatus
{
    public interface ISettings
    {
        string ClientId { get; set; }

        Item RootItem { get; }

        bool DebugMode { get; set; }

        bool ReportAnonymousUsage { get; set; }

        bool ShowStatusIcons { get; set; }

        bool ShowStatusColors { get; set; }

        Theme Theme { get; set; }
    }
}
