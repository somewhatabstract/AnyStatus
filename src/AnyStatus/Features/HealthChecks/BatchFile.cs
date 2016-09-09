using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Media;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus.Models
{
    [DisplayName("Batch File")]
    [Description("Execute a batch file.")]
    public class BatchFile : Item, IScheduledItem
    {
        public BatchFile()
        {
            Timeout = 1;
        }

        [PropertyOrder(10)]
        [Category("Batch File")]
        [DisplayName("File Name")]
        [Description("The batch file full path")]
        [Editor(typeof(FileEditor), typeof(FileEditor))]
        public string FileName { get; set; }

        [PropertyOrder(20)]
        [Category("Batch File")]
        [Description("The batch file arguments")]
        public string Arguments { get; set; }

        [PropertyOrder(30)]
        [Category("Batch File")]
        [DisplayName("Timeout (minutes)")]
        [Description("The script execution timeout in minutes")]
        public int Timeout { get; set; }
    }

    public class BatchFileHandler : IHandler<BatchFile>
    {
        public void Handle(BatchFile item)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = item.FileName,
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
