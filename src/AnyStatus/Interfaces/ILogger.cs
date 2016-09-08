using System;

namespace AnyStatus
{
    public interface ILogger
    {
        void Info(string message);

        void Error(Exception exception, string message);
    }
}
