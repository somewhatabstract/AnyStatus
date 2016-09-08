using System;

namespace AnyStatus
{
    internal class NullLogger : ILogger
    {
        public void Error(Exception exception, string message)
        {
        }

        public void Info(string message)
        {
        }
    }
}