using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus.Models
{
    [CategoryOrder("Batch File", 10)]
    [DisplayName("Batch File")]
    [Description("Execute a batch file.")]
    public class BatchFile : Item, IScheduledItem
    {
        public BatchFile()
        {
            Timeout = 1;
        }

        [Required]
        [PropertyOrder(10)]
        [Category("Batch File")]
        [DisplayName("File Name")]
        [Description("The batch file path")]
        [Editor(typeof(FileEditor), typeof(FileEditor))]
        public string FileName { get; set; }

        [PropertyOrder(20)]
        [Category("Batch File")]
        [Description("The batch file arguments")]
        public string Arguments { get; set; }

        [PropertyOrder(30)]
        [Category("Batch File")]
        [Description("The script execution timeout in minutes")]
        public int Timeout { get; set; }
    }

    public class BatchFileHandler : IHandler<BatchFile>
    {
        [DebuggerStepThrough]
        public void Handle(BatchFile item)
        {
            if (!File.Exists(item.FileName))
                throw new FileNotFoundException(item.FileName);

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

            item.State = (process.ExitCode == 0) ? ItemState.Ok : ItemState.Failed;
        }
    }
}
