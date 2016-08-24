using AnyStatus.Features.Edit;
using AnyStatus.Models;
using AnyStatus.Views;

namespace AnyStatus.Interfaces
{
    public interface IViewLocator
    {
        /// <summary>
        /// New item window
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        NewWindow NewWindow(Item parent);

        /// <summary>
        /// Edit item window
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        EditWindow EditWindow(Item parent);
    }
}