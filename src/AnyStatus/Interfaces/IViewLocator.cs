using AnyStatus.Models;
using AnyStatus.Views;

namespace AnyStatus.Interfaces
{
    public interface IViewLocator
    {
        NewItemDialog NewStatusDialog(Item parent);
    }
}