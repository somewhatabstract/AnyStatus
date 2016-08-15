using AnyStatus.Models;
using AnyStatus.Views;

namespace AnyStatus.Interfaces
{
    public interface IViewLocator
    {
        NewStatusDialog NewStatusDialog(Item parent);
    }
}