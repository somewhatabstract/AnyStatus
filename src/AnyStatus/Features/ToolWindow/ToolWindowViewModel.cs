﻿using AnyStatus.Infrastructure;
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
        private ILogger _logger;
        private IUserSettings _userSettings;
        private IViewLocator _viewLocator;
        private IJobScheduler _jobScheduler;

        public ToolWindowViewModel(IJobScheduler jobScheduler,
                                    IUserSettings userSettings,
                                    IViewLocator viewLocator,
                                    ILogger logger)
        {
            if (jobScheduler == null)
                throw new ArgumentNullException(nameof(jobScheduler));
            if (userSettings == null)
                throw new ArgumentNullException(nameof(userSettings));
            if (viewLocator == null)
                throw new ArgumentNullException(nameof(viewLocator));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _jobScheduler = jobScheduler;
            _userSettings = userSettings;
            _viewLocator = viewLocator;
            _logger = logger;

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
            _userSettings.SettingsReset += (s, e) =>
            {
                OnPropertyChanged("RootItem");
            };

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
                    _logger.Info("Could not add a new folder. Exception: " + ex.ToString());
                }
            });

            DeleteCommand = new RelayCommand(p =>
            {
                try
                {
                    var result = MessageBox.Show("Are you sure?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Asterisk);

                    if (result != MessageBoxResult.Yes) return;

                    var selectedItem = p as Item;

                    if (selectedItem == null)
                        return;

                    if (selectedItem is IScheduledItem)
                    {
                        JobManager.RemoveJob(selectedItem.Id.ToString());
                    }

                    selectedItem.Delete();

                    _userSettings.Save();
                }
                catch (Exception ex)
                {
                    _logger.Info("Could not delete item. Exception: " + ex.ToString());
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
                    _logger.Info("Could not open a new item modal dialog. Exception: " + ex.ToString());
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
                    _logger.Info("Could not open item properties modal dialog. Exception: " + ex.ToString());
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
                    _logger.Info("Could not enable an item. Exception: " + ex.ToString());
                }
            });

            DisableItemCommand = new RelayCommand(p =>
            {
                try
                {
                    var item = p as Item;

                    if (item == null)
                        return;

                    if (item is IScheduledItem)
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
                    _logger.Info("Could not save. Exception: " + ex.ToString());
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
                    _logger.Error(ex, "Could not refresh item.");
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
                    _logger.Info("Could not move item up. Exception: " + ex.ToString());
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
                    _logger.Info("Could not move item down. Exception: " + ex.ToString());
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


