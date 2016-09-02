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

        public ToolWindowViewModel(IUserSettings userSettings, IViewLocator viewLocator)
        {
            if (userSettings == null)
                throw new ArgumentNullException(nameof(userSettings));

            if (viewLocator == null)
                throw new ArgumentNullException(nameof(viewLocator));

            _userSettings = userSettings;
            _viewLocator = viewLocator;

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

                    var item = new Folder
                    {
                        Name = "New Folder",
                        IsEditing = true
                    };

                    item.Parent = selectedItem;
                    selectedItem.Items.Add(item);
                    selectedItem.IsExpanded = true;

                    _userSettings.Save();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
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

                    if (selectedItem.Parent != null)
                    {
                        selectedItem.Parent.Items.Remove(selectedItem);
                    }
                    else
                    {
                        _userSettings.RootItem.Items.Remove(selectedItem);
                    }

                    if (!(selectedItem is Folder))
                    {
                        JobManager.RemoveJob(selectedItem.Id.ToString());
                    }

                    _userSettings.Save();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
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
                    Debug.WriteLine(ex);
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
                    Debug.WriteLine(ex);
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
                    Debug.WriteLine(ex);
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
                    Debug.WriteLine(ex);
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
                    Debug.WriteLine(ex);
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


