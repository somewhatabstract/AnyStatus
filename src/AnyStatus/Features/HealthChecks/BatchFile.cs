using System.ComponentModel;

namespace AnyStatus.Models
{
    [Browsable(false)]
    [DisplayName("Batch File")]
    [Description("Execute a batch file")]
    public class BatchFile : Item, IScheduledItem
    {
        [DisplayName("File Path")]
        [Description("The batch file path")]
        [Category("Batch File")]
        public string FilePath { get; set; }
    }

    public class BatchFileHandler : IHandler<BatchFile>
    {
        public void Handle(BatchFile item)
        {

        }
    }
}
