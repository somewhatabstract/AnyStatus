using AnyStatus.Infrastructure;
using AnyStatus.Interfaces;
using AnyStatus.Models;
using FluentScheduler;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace AnyStatus.ViewModels
{
    public class ToolWindowViewModel : INotifyPropertyChanged
    {
        private readonly ILogger _logger;
        private readonly IUserSettings _userSettings;
        private readonly IViewLocator _viewLocator;
        private readonly IJobScheduler _jobScheduler;

        public ToolWindowViewModel(IJobScheduler jobScheduler,
                                   IUserSettings userSettings,
                                   IViewLocator viewLocator,
                                   ILogger logger)
        {
            _jobScheduler = Preconditions.CheckNotNull(jobScheduler, nameof(jobScheduler));
            _userSettings = Preconditions.CheckNotNull(userSettings, nameof(userSettings));
            _viewLocator = Preconditions.CheckNotNull(viewLocator, nameof(viewLocator));
            _logger = Preconditions.CheckNotNull(logger, nameof(logger));

            Initialize();
        }

        /// <summary>
        /// The tree-view root item.
        /// </summary>
        public Item RootItem
        {
            get { return _userSettings.RootItem; }
        }

        private void Initialize()
        {
            _userSettings.PropertyChanged += (s, e) => OnPropertyChanged(e.PropertyName);

            AddFolderCommand = new RelayCommand(p =>
            {
                try
                {
                    var selectedItem = p as Item ?? _userSettings.RootItem;

                    if (selectedItem == null) return;

                    var folder = new Folder
                    {
                        Name = "New Folder",
                        IsEditing = true
                    };

                    selectedItem.Add(folder);
                    selectedItem.IsExpanded = true;

                    _userSettings.Save();
                }
                catch (Exception ex)
                {
                    _logger.Info("Failed to add a new folder. Exception: " + ex.ToString());
                }
            });

            DeleteCommand = new RelayCommand(p =>
            {
                var selectedItem = p as Item;

                if (selectedItem == null)
                    return;

                var result = MessageBox.Show("Are you sure?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Asterisk);

                if (result != MessageBoxResult.Yes)
                    return;

                try
                {
                    if (selectedItem is IScheduledItem)
                    {
                        _jobScheduler.Remove(selectedItem);
                    }

                    selectedItem.Delete();

                    _userSettings.Save();
                }
                catch (Exception ex)
                {
                    _logger.Info("Failed to delete item. Exception: " + ex.ToString());
                }
            });

            DuplicateCommand = new RelayCommand(p =>
            {
                //todo: remove duplication with new item modal

                var selectedItem = p as Item;

                if (selectedItem == null) return;

                try
                {
                    var item = selectedItem.Duplicate();

                    _userSettings.Save();

                    _jobScheduler.Schedule(item);
                }
                catch (Exception ex)
                {
                    _logger.Info("Failed to duplicate item. Exception: " + ex.ToString());
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

            EnableItemCommand = new RelayCommand(p =>
            {
                try
                {
                    var item = p as Item;

                    if (item == null)
                        return;

                    if (item is IScheduledItem)
                    {
                        JobManager.RemoveJob(item.Id.ToString());

                        var job = TinyIoCContainer.Current.Resolve<ScheduledJob>();
                        job.Item = item;

                        JobManager.AddJob(job, s =>
                            s.WithName(item.Id.ToString()).ToRunNow().AndEvery(item.Interval).Minutes());
                    }

                    item.IsEnabled = true;

                    _userSettings.Save();
                }
                catch (Exception ex)
                {
                    _logger.Info("Failed to enable an item. Exception: " + ex.ToString());
                }
            });

            DisableItemCommand = new RelayCommand(p =>
            {
                try
                {
                    var item = p as Item;

                    if (item == null) return;

                    if (item is IScheduledItem)
                        JobManager.RemoveJob(item.Id.ToString());

                    item.IsEnabled = false;

                    _userSettings.Save();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            });

            SaveCommand = new RelayCommand(p =>
            {
                try
                {
                    _userSettings.Save();
                }
                catch (Exception ex)
                {
                    _logger.Info("Failed to save. Exception: " + ex.ToString());
                }
            });

            RefreshItemCommand = new RelayCommand(p =>
            {
                try
                {
                    _jobScheduler.Execute(p as Item);
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

                    _userSettings.Save();
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

                    _userSettings.Save();
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


