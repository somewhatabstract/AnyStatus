using System;

namespace AnyStatus
{
    public interface ILogger
    {
        bool IsEnabled { get; set; }

        void Info(string message);

        void Error(Exception exception, string message);
    }
}
