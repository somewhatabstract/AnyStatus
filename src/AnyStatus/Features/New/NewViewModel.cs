using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace AnyStatus
{
    public class NewViewModel : NotifyPropertyChanged
    {
        private string _message;
        private bool _canTest = true;
        private Template _selectedTemplate;
        private readonly IMediator _mediator;

        public event EventHandler CloseRequested;

        public NewViewModel(IMediator mediator, IEnumerable<Template> templates)
        {
            _mediator = Preconditions.CheckNotNull(mediator, nameof(mediator));
            Templates = Preconditions.CheckNotNull(templates, nameof(templates));

            Initialize();
        }

        private void Initialize()
        {
            AddCommand = new RelayCommand(item => _mediator.TrySend(new AddCommand(item as Item, Parent, RequestClose)));

            TestCommand = new RelayCommand(item =>
                _mediator.TrySend(new TestCommand(item as Item, 
                canExecute => CanTest = canExecute, 
                message => Message = message)));

            CancelCommand = new RelayCommand(p => RequestClose());

            SelectedTemplate = Templates?.FirstOrDefault();
        }

        private void RequestClose()
        {
            CloseRequested?.Invoke(this, EventArgs.Empty);
        }

        #region Properties

        public Template SelectedTemplate
        {
            get
            {
                return _selectedTemplate;
            }
            set
            {
                _selectedTemplate = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<Template> Templates { get; set; }

        public Item Parent { get; set; }

        public bool CanTest
        {
            get { return _canTest; }
            set { _canTest = value; OnPropertyChanged(); }
        }

        public string Message
        {
            get { return _message; }
            set { _message = value; OnPropertyChanged(); }
        }

        #endregion

        #region Commands

        public ICommand AddCommand { get; set; }

        public ICommand TestCommand { get; set; }

        public ICommand CancelCommand { get; set; }

        #endregion
    }
}