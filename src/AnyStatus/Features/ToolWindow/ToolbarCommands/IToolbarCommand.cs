using System.ComponentModel.Design;

namespace AnyStatus
{
    public interface IToolbarCommand
    {
        MenuCommand MenuCommand { get; }
    }
}
