using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace AnyStatus
{
    public class ToolWindowViewModel : INotifyPropertyChanged
    {
        private readonly ILogger _logger;
        private readonly ISettingsStore _settingsStore;
        private readonly IViewLocator _viewLocator;
        private readonly IJobScheduler _jobScheduler;

        public ToolWindowViewModel(IJobScheduler jobScheduler,
                                   ISettingsStore settingsStore,
                                   IViewLocator viewLocator,
                                   ILogger logger)
        {
            _jobScheduler = Preconditions.CheckNotNull(jobScheduler, nameof(jobScheduler));
            _settingsStore = Preconditions.CheckNotNull(settingsStore, nameof(settingsStore));
            _viewLocator = Preconditions.CheckNotNull(viewLocator, nameof(viewLocator));
            _logger = Preconditions.CheckNotNull(logger, nameof(logger));

            Initialize();
        }

        public AppSettings Settings
        {
            get
            {
                return _settingsStore.Settings;
            }
        }

        private void Initialize()
        {
            _settingsStore.PropertyChanged += (s, e) => OnPropertyChanged(e.PropertyName);

            AddFolderCommand = new RelayCommand(p =>
            {
                try
                {
                    var item = p as Item ?? _settingsStore.Settings.RootItem;

                    if (item == null) return;

                    var folder = new Folder
                    {
                        Name = "New Folder",
                        IsEditing = true
                    };

                    item.Add(folder);
                    item.IsExpanded = true;

                    _settingsStore.TrySave();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Failed to add a new folder.");
                }
            });

            DeleteCommand = new DeleteCommand(_jobScheduler, _settingsStore, _logger);

            DuplicateCommand = new RelayCommand(p =>
            {
                //todo: remove duplication with new item modal

                var item = p as Item;

                if (item == null) return;

                try
                {
                    var clone = item.Duplicate();

                    _jobScheduler.Schedule(clone);

                    _settingsStore.TrySave();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Failed to duplicate item.");
                }
            });

            AddItemCommand = new RelayCommand(p =>
            {
                try
                {
                    var selectedItem = p as Item;

                    var dlg = _viewLocator.NewWindow(selectedItem);

                    dlg.ShowModal();
                }
                catch (Exception ex)
                {
                    _logger.Info("Failed to open a new item modal dialog. Exception: " + ex.ToString());
                }
            });

            EditItemCommand = new RelayCommand(p =>
            {
                try
                {
                    var selectedItem = p as Item;

                    var dlg = _viewLocator.EditWindow(selectedItem);

                    dlg.ShowModal();
                }
                catch (Exception ex)
                {
                    _logger.Info("Failed to open item properties modal dialog. Exception: " + ex.ToString());
                }
            });

            RenameCommand = new RelayCommand(p =>
            {
                var item = p as Item;

                if (item == null)
                    return;

                item.IsEditing = true;
            });

            EnableItemCommand = new EnableCommand(_settingsStore, _logger);

            DisableItemCommand = new DisableCommand(_settingsStore, _logger);

            SaveCommand = new RelayCommand(p => { _settingsStore.TrySave(); });

            RefreshItemCommand = new RelayCommand(p =>
            {
                var item = p as Item;

                if (item == null) return;

                try
                {
                    _jobScheduler.Execute(item);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Failed to refresh item");
                }
            });

            MoveUpCommand = new RelayCommand(
            p =>
            {
                try
                {
                    var item = p as Item;

                    if (item == null)
                        return;

                    item.MoveUp();

                    _settingsStore.TrySave();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Failed to move item up");
                }
            },
            p =>
            {
                var item = p as Item;

                return item != null && item.CanMoveUp();
            });

            MoveDownCommand = new RelayCommand(
            p =>
            {
                try
                {
                    var item = p as Item;

                    if (item == null)
                        return;

                    item.MoveDown();

                    _settingsStore.TrySave();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Failed to move item down");
                }
            },
            p =>
            {
                var item = p as Item;

                return item != null && item.CanMoveDown();
            });
        }

        #region Commands

        public ICommand AddFolderCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand RenameCommand { get; set; }
        public ICommand AddItemCommand { get; set; }
        public ICommand EditItemCommand { get; set; }
        public ICommand DuplicateCommand { get; set; }
        public ICommand RefreshItemCommand { get; set; }
        public ICommand EnableItemCommand { get; set; }
        public ICommand DisableItemCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand MoveUpCommand { get; set; }
        public ICommand MoveDownCommand { get; set; }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}


