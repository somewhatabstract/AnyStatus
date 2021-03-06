﻿using System.ComponentModel;
using System.Windows.Input;

namespace AnyStatus
{
    public class ToolWindowViewModel : NotifyPropertyChanged
    {
        private readonly IMediator _mediator;
        private readonly ISettingsStore _settingsStore;

        public ToolWindowViewModel(IMediator mediator, ISettingsStore settingsStore)
        {
            _mediator = Preconditions.CheckNotNull(mediator, nameof(mediator));
            _settingsStore = Preconditions.CheckNotNull(settingsStore, nameof(settingsStore));

            Initialize();
        }

        public UserSettings Settings
        {
            get { return _settingsStore.Settings; }
        }

        private void Initialize()
        {
            _settingsStore.PropertyChanged += SettingsStore_PropertyChanged;

            OpenInBrowserCommand = new RelayCommand(item => _mediator.TrySend(item, typeof(IOpenInBrowser<>)));

            TriggerBuildCommand =
                new RelayCommand(async item => await _mediator.TrySendAsync(item, typeof(ITriggerBuild<>))
                        .ContinueWith(task => _mediator.TrySend(new RefreshCommand(item as Item))));

            AddFolderCommand = new RelayCommand(item => _mediator.TrySend(new AddFolderCommand(item as Item)));

            DeleteCommand = new RelayCommand(item => _mediator.TrySend(new DeleteCommand(item as Item)));

            DuplicateCommand = new RelayCommand(item => _mediator.TrySend(new DuplicateCommand(item as Item)));

            ShowAddDialogCommand = new RelayCommand(item => _mediator.TrySend(new ShowAddDialogCommand(item as Item)));

            ShowEditDialogCommand = new RelayCommand(item => _mediator.TrySend(new ShowEditDialogCommand(item as Item)));

            EnableCommand = new RelayCommand(item => _mediator.TrySend(new EnableCommand(item as Item)));

            DisableCommand = new RelayCommand(item => _mediator.TrySend(new DisableCommand(item as Item)));

            SaveCommand = new RelayCommand(item => _settingsStore.TrySave());

            RefreshCommand = new RelayCommand(item => _mediator.TrySend(new RefreshCommand(item as Item)));

            RenameCommand = new RelayCommand(p =>
            {
                var item = p as Item;

                if (item != null)
                    item.IsEditing = true;
            });

            MoveUpCommand = new RelayCommand(
                item => _mediator.TrySend(new MoveUpCommand(item as Item)),
                item => (item as Item) != null && (item as Item).CanMoveUp());

            MoveDownCommand = new RelayCommand(
                item => _mediator.TrySend(new MoveDownCommand(item as Item)),
                item => (item as Item) != null && (item as Item).CanMoveDown());

            StartWindowsServiceCommand = new RelayCommand(async item => await _mediator.TrySendAsync(item, typeof(IStartWindowsService<>)));

            StopWindowsServiceCommand = new RelayCommand(async item => await _mediator.TrySendAsync(item, typeof(IStopWindowsService<>)));

            RestartWindowsServiceCommand = new RelayCommand(async item => await _mediator.TrySendAsync(item, typeof(IRestartWindowsService<>)));
        }

        private void SettingsStore_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(Settings)))
            {
                OnPropertyChanged(nameof(Settings));
            }
        }
        
        #region Commands

        public ICommand OpenInBrowserCommand { get; set; }

        public ICommand TriggerBuildCommand { get; set; }

        public ICommand AddFolderCommand { get; set; }

        public ICommand DeleteCommand { get; set; }

        public ICommand RenameCommand { get; set; }

        public ICommand ShowAddDialogCommand { get; set; }

        public ICommand ShowEditDialogCommand { get; set; }

        public ICommand DuplicateCommand { get; set; }

        public ICommand RefreshCommand { get; set; }

        public ICommand EnableCommand { get; set; }

        public ICommand DisableCommand { get; set; }

        public ICommand SaveCommand { get; set; }

        public ICommand MoveUpCommand { get; set; }

        public ICommand MoveDownCommand { get; set; }

        public ICommand StartWindowsServiceCommand { get; set; }

        public ICommand StopWindowsServiceCommand { get; set; }

        public ICommand RestartWindowsServiceCommand { get; set; } 

        #endregion
    }
}


