using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Diagnostics;

namespace AnyStatus.Infrastructure
{
    public class Logger : ILogger
    {
        private Guid _guid;
        private IVsOutputWindowPane _pane;
        private IServiceProvider _provider;
        private object _syncRoot = new object();

        private const string paneName = "AnyStatus";

        public bool IsEnabled { get; set; } = true;

        public Logger(IServiceProvider provider)
        {
            _provider = provider;
        }

        public void Info(string message)
        {
            Log(message);
        }

        public void Error(Exception exception, string message)
        {
            Log($"{message}\r\nException: {exception.Message}\r\nInner Exception: {exception.InnerException?.Message}");
        }

        private bool EnsurePane()
        {
            if (_pane == null)
            {
                lock (_syncRoot)
                {
                    if (_pane == null)
                    {
                        _guid = Guid.NewGuid();
                        IVsOutputWindow output = (IVsOutputWindow)_provider.GetService(typeof(SVsOutputWindow));
                        output.CreatePane(ref _guid, paneName, 1, 1);
                        output.GetPane(ref _guid, out _pane);
                    }
                }
            }

            return _pane != null;
        }

        private void Log(string message)
        {
            if (string.IsNullOrEmpty(message))
                return;

            try
            {
                if (IsEnabled && EnsurePane())
                    _pane.OutputStringThreadSafe(
                        $"[{DateTime.Now}] {message}{Environment.NewLine}");
            }
            catch (Exception ex)
            {
                Debug.Write(ex);
            }
        }
    }
}
