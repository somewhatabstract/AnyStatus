using System;
using System.Windows.Input;

namespace AnyStatus
{
    public class EditViewModel
    {
        private Item _item;
        private Item _sourceItem;

        private readonly IMediator _mediator;

        public event EventHandler CloseRequested;

        public EditViewModel(IMediator mediator)
        {
            _mediator = Preconditions.CheckNotNull(mediator, nameof(mediator));

            Initialize();
        }

        private void Initialize()
        {
            SaveCommand = new RelayCommand(item => _mediator.TrySend(new EditCommand(item as Item, SourceItem, RequestClose)));

            CancelCommand = new RelayCommand(p => RequestClose());
        }

        private void RequestClose()
        {
            CloseRequested?.Invoke(this, EventArgs.Empty);
        }

        public Item Item
        {
            get
            {
                return _item;
            }
            set
            {
                _item = value.Clone() as Item;
                _sourceItem = value;
            }
        }

        public Item SourceItem { get { return _sourceItem; } }

        #region Commands

        public ICommand SaveCommand { get; set; }

        public ICommand CancelCommand { get; set; }

        #endregion
    }
}
