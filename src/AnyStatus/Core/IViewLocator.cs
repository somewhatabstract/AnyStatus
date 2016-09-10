using AnyStatus.Features.Edit;
using AnyStatus.Models;
using AnyStatus.Views;

namespace AnyStatus
{
    public interface IViewLocator
    {

        NewWindow NewWindow(Item parent);

        EditWindow EditWindow(Item parent);

        //OptionsDialogControl OptionsWindow();
    }
}