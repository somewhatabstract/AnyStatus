using System.ComponentModel;

namespace AnyStatus.Models
{
    [Browsable(false)]
    public class Folder : Item
    {
        [Browsable(false)]
        public new int Interval { get; set; }
    }
}
