using System;
using System.Diagnostics;

namespace AnyStatus
{
    public class ProcessStarter : IProcessStarter
    {
        private readonly ILogger _logger;

        public ProcessStarter(ILogger logger)
        {
            _logger = Preconditions.CheckNotNull(logger, nameof(logger));
        }

        public void Start(string fileName)
        {
            Process.Start(fileName);
        }

        public int Start(ProcessStartInfo info, TimeSpan timeout)
        {
            var process = Process.Start(info);

            process.WaitForExit((int)timeout.TotalMilliseconds);

            var output = process.StandardOutput.ReadToEnd();

            _logger.Info(output);

            return process.ExitCode;
        }
    }
}
