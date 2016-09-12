using AnyStatus.Features.Options;
using AnyStatus.Infrastructure;
using Microsoft.VisualStudio.Shell;
using System.Windows;

namespace AnyStatus.Views
{
    public class ImportExport : UIElementDialogPage
    {
        private ImportExportControl _view;

        public ImportExport() : this(TinyIoCContainer.Current.Resolve<ImportExportControl>())
        {
        }

        public ImportExport(ImportExportControl view)
        {
            _view = view;
        }

        protected override UIElement Child
        {
            get
            {
                return _view;
            }
        }
    }
}