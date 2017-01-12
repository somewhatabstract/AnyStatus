using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace AnyStatus
{
    [CategoryOrder("Batch File", 10)]
    [DisplayName("Batch File")]
    [Description("Test the exit code of a batch file")]
    public class BatchFile : Item, IScheduledItem
    {
        private const string Category = "Batch File";

        public BatchFile()
        {
            Timeout = 1;
        }

        [Required]
        [PropertyOrder(10)]
        [Category(Category)]
        [DisplayName("File Name")]
        [Description("The batch file path")]
        [Editor(typeof(FileEditor), typeof(FileEditor))]
        public string FileName { get; set; }

        [PropertyOrder(20)]
        [Category(Category)]
        [Description("The batch file arguments")]
        public string Arguments { get; set; }

        [PropertyOrder(30)]
        [Category(Category)]
        [Description("The script execution timeout in minutes")]
        public int Timeout { get; set; }
    }

    public class BatchFileMonitor : IMonitor<BatchFile>
    {
        private readonly IProcessStarter _processStarter;

        public BatchFileMonitor(IProcessStarter processStarter)
        {
            _processStarter = Preconditions.CheckNotNull(processStarter, nameof(processStarter));
        }

        [DebuggerStepThrough]
        public void Handle(BatchFile item)
        {
            Validate(item);

            var info = new ProcessStartInfo
            {
                FileName = item.FileName,
                Arguments = item.Arguments,
                CreateNoWindow = true,
                ErrorDialog = false,
                LoadUserProfile = false,
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = false,
                RedirectStandardOutput = true,
            };

            var exitCode = _processStarter.Start(info, TimeSpan.FromMinutes(item.Timeout));

            item.State = exitCode == 0 ? State.Ok : State.Failed;
        }

        [DebuggerStepThrough]
        private static void Validate(BatchFile item)
        {
            if (!File.Exists(item.FileName))
                throw new FileNotFoundException(item.FileName);
        }
    }
}
