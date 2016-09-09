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
        }

        [Required]
        [PropertyOrder(10)]
        [Category("PowerShell")]
        [DisplayName("File Name")]
        [Description("The script file path")]
        [Editor(typeof(FileEditor), typeof(FileEditor))]
        public string FileName { get; set; }

        [PropertyOrder(20)]
        [Category("PowerShell")]
        [Description("The script arguments")]
        public string Arguments { get; set; }

        [PropertyOrder(30)]
        [Category("PowerShell")]
        [Description("The script execution timeout in minutes")]
        public int Timeout { get; set; }
    }

    public class PowerShellScriptHandler : IHandler<PowerShellScript>
    {
        public void Handle(PowerShellScript item)
        {
            if (!File.Exists(item.FileName))
                throw new FileNotFoundException(item.FileName);

            var startInfo = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"&'{item.FileName}' {item.Arguments}",
                CreateNoWindow = true,
                ErrorDialog = false,
                LoadUserProfile = false,
                WindowStyle = ProcessWindowStyle.Hidden,
            };

            var process = Process.Start(startInfo);

            process.WaitForExit(item.Timeout * 60 * 1000);

            item.Brush = process.ExitCode == 0 ? Brushes.Green : Brushes.Red;
        }
    }
}
