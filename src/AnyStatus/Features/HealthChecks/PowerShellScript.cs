using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Windows.Media;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus.Models
{
    [DisplayName("PowerShell Script")]
    [Description("Execute a PowerShell script")]
    public class PowerShellScript : Item, IScheduledItem
    {
        public PowerShellScript()
        {
            Timeout = 1;
            BypassExecutionPolicy = true;
        }

        [Required]
        [PropertyOrder(10)]
        [Category("PowerShell Script")]
        [DisplayName("File Name")]
        [Description("The script file path")]
        [Editor(typeof(FileEditor), typeof(FileEditor))]
        public string FileName { get; set; }

        [PropertyOrder(20)]
        [Category("PowerShell Script")]
        [Description("The script arguments")]
        public string Arguments { get; set; }

        [PropertyOrder(30)]
        [Category("PowerShell Script")]
        [Description("The script execution timeout in minutes")]
        public int Timeout { get; set; }

        [ReadOnly(true)]
        [PropertyOrder(40)]
        [Category("PowerShell Script")]
        [DisplayName("Bypass Execution Policy")]
        [Description("Bypass PowerShell execution policy")]
        public bool BypassExecutionPolicy { get; set; }
    }

    public class PowerShellScriptHandler : IHandler<PowerShellScript>
    {
        private ILogger _logger;

        public PowerShellScriptHandler(ILogger logger)
        {
            _logger = logger;
        }

        [DebuggerStepThrough]
        public void Handle(PowerShellScript item)
        {
            if (!File.Exists(item.FileName))
                throw new FileNotFoundException(item.FileName);

            var executionPolicy = item.BypassExecutionPolicy ? "ByPass" : "Restricted";

            var startInfo = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-ExecutionPolicy {executionPolicy} -File \"{item.FileName}\" {item.Arguments}",
                CreateNoWindow = true,
                ErrorDialog = false,
                LoadUserProfile = false,
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = false,
                RedirectStandardOutput = true,
            };

            var process = Process.Start(startInfo);

            process.WaitForExit(item.Timeout * 60 * 1000);

            var output = process.StandardOutput.ReadToEnd();

            _logger.Info(output);

            item.Brush = process.ExitCode == 0 ? Brushes.Green : Brushes.Red;
        }
    }
}
