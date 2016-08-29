using System.ComponentModel;

namespace AnyStatus.Models
{
    public class Folder : Item
    {
        [Browsable(false)]
        public new int Interval { get; set; }
    }
}
