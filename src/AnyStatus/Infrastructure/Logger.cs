using AnyStatus.Interfaces;
using Microsoft.VisualStudio.Shell.Interop;
using System;

namespace AnyStatus.Infrastructure
{
    public class Logger : ILogger
    {
        private IVsOutputWindowPane pane;
        private IServiceProvider _provider;
        private Guid _guid;
        private string _name;
        private object _syncRoot = new object();

        public Logger(IServiceProvider provider)
        {
            _provider = provider;
            _name = "AnyStatus";
        }

        public void Log(string message)
        {
            if (string.IsNullOrEmpty(message))
                return;

            try
            {
                if (EnsurePane())
                {
                    pane.OutputStringThreadSafe(DateTime.Now + ": " + message + Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
            }
        }

        private bool EnsurePane()
        {
            if (pane == null)
            {
                lock (_syncRoot)
                {
                    if (pane == null)
                    {
                        _guid = Guid.NewGuid();
                        IVsOutputWindow output = (IVsOutputWindow)_provider.GetService(typeof(SVsOutputWindow));
                        output.CreatePane(ref _guid, _name, 1, 1);
                        output.GetPane(ref _guid, out pane);
                    }
                }
            }

            return pane != null;
        }
    }
}
