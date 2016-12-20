using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Diagnostics;

namespace AnyStatus.VSPackage
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

        public void Error(Exception exception)
        {
            Log(exception.ToString());
        }

        public void Error(Exception exception, string message)
        {
            Log($"{message}. Exception:\r\n{exception}");
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
            if (!IsEnabled || string.IsNullOrEmpty(message) || !EnsurePane())
                return;

            try
            {
                _pane.OutputStringThreadSafe(
                    $"[{DateTime.Now.ToString("HH:mm:ss.fff")}] {message}{Environment.NewLine}");
            }
            catch (Exception ex)
            {
                Debug.Write(ex);
            }
        }
    }
}
