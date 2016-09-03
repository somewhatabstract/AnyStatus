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
        private IUserSettings _userSettings;
        private IViewLocator _viewLocator;
        private ILogger _logger;

        public ToolWindowViewModel(IUserSettings userSettings, IViewLocator viewLocator, ILogger logger)
        {
            if (userSettings == null)
                throw new ArgumentNullException(nameof(userSettings));
            if (viewLocator == null)
                throw new ArgumentNullException(nameof(viewLocator));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _userSettings = userSettings;
            _viewLocator = viewLocator;
            _logger = logger;

            Initialize();
        }

        public Item RootItem { get { return _userSettings.RootItem; } }

        private void Initialize()
        {
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
                    _logger.Log("Failed to add a new folder. Exception: " + ex.ToString());
                }
            });

            RemoveCommand = new RelayCommand(p =>
            {
                try
                {
                    var result = MessageBox.Show("Are you sure?", "Remove", MessageBoxButton.YesNo, MessageBoxImage.Asterisk);

                    if (result != MessageBoxResult.Yes) return;

                    var selectedItem = p as Item;

                    if (selectedItem == null)
                    {
                        return;
                    }

                    if (selectedItem is IScheduledItem)
                    {
                        JobManager.RemoveJob(selectedItem.Id.ToString());
                    }

                    selectedItem.Remove();

                    _userSettings.Save();
                }
                catch (Exception ex)
                {
                    _logger.Log("Failed to remove an item. Exception: " + ex.ToString());
                }
            });

            AddItemCommand = new RelayCommand(p =>
            {
                try
                {
                    var selectedItem = p as Item;

                    var dlg = _viewLocator.NewWindow(selectedItem);

                    dlg.ShowDialog();
                }
                catch (Exception ex)
                {
                    _logger.Log("Failed to open a new item window. Exception: " + ex.ToString());
                }
            });

            EditItemCommand = new RelayCommand(p =>
            {
                try
                {
                    var selectedItem = p as Item;

                    var dlg = _viewLocator.EditWindow(selectedItem);

                    dlg.ShowDialog();
                }
                catch (Exception ex)
                {
                    _logger.Log("Failed to open item properties window. Exception: " + ex.ToString());
                }
            });

            RenameCommand = new RelayCommand(p =>
            {
                var item = p as Item;

                if (item != null)
                {
                    item.IsEditing = true;
                }
            });

            EnableItemCommand = new RelayCommand(p =>
            {
                try
                {
                    var item = p as Item;

                    if (item == null)
                        return;

                    JobManager.RemoveJob(item.Id.ToString());

                    JobManager.AddJob(new ScheduledJob(item),
                       schedule => schedule.WithName(item.Id.ToString()).ToRunNow().AndEvery(item.Interval).Minutes());

                    item.IsEnabled = true;

                    _userSettings.Save();
                }
                catch (Exception ex)
                {
                    _logger.Log("Failed to enable an item. Exception: " + ex.ToString());
                }
            });

            DisableItemCommand = new RelayCommand(p =>
            {
                try
                {
                    var item = p as Item;

                    if (item == null)
                        return;

                    JobManager.RemoveJob(item.Id.ToString());

                    item.IsEnabled = false;

                    item.Brush = Brushes.Silver;

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
                    _logger.Log("Failed to save. Exception: " + ex.ToString());
                }
            });

            RefreshItemCommand = new RelayCommand(p =>
            {
                try
                {
                    var item = p as Item;

                    if (item == null)
                    {
                        return;
                    }

                    var job = JobManager.GetSchedule(item.Id.ToString());

                    job.Execute();
                }
                catch (Exception ex)
                {
                    _logger.Log("Failed to refresh an item. Exception: " + ex.ToString());
                }
            });
        }

        #region Commands

        public ICommand AddFolderCommand { get; set; }
        public ICommand RemoveCommand { get; set; }
        public ICommand RenameCommand { get; set; }

        public ICommand AddItemCommand { get; set; }
        public ICommand RemoveItemCommand { get; set; }
        public ICommand EditItemCommand { get; set; }
        public ICommand RefreshItemCommand { get; set; }
        public ICommand EnableItemCommand { get; set; }
        public ICommand DisableItemCommand { get; set; }

        //public ICommand ReparentItemCommand { get; set; }

        public ICommand SaveCommand { get; set; }

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


