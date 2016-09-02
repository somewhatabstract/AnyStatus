using System;
using AnyStatus.Interfaces;

namespace AnyStatus
{
    internal class NullLogger : ILogger
    {
        public void Log(string message)
        {
        }
    }
}