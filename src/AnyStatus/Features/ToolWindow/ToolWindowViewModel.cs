using AnyStatus.Interfaces;
using AnyStatus.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace AnyStatus.ViewModels
{
    public class ToolWindowViewModel : INotifyPropertyChanged
    {
        private IUserSettings _userSettings;
        private IViewLocator _viewLocator;

        public ToolWindowViewModel(IUserSettings userSettings, IViewLocator viewLocator)
        {
            if (userSettings == null)
            {
                throw new ArgumentNullException(nameof(userSettings));
            }

            if (viewLocator == null)
            {
                throw new ArgumentNullException(nameof(viewLocator));
            }

            _userSettings = userSettings;
            _viewLocator = viewLocator;

            Initialize();
        }

        public ObservableCollection<Item> Items
        {
            get
            {
                return _userSettings.Items;
            }
        }

        private void Initialize()
        {
            AddFolderCommand = new RelayCommand(p =>
            {
                var selectedItem = p as Item;

                var item = new Folder()
                {
                    Name = "New Folder"
                };

                if (selectedItem != null)
                {
                    item.Parent = selectedItem;
                    selectedItem.Items.Add(item);
                    selectedItem.IsExpanded = true;
                }
                else
                {
                    _userSettings.Items.Add(item);
                }

                _userSettings.Save();
            });

            RemoveFolderCommand = new RelayCommand(p =>
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
                    _userSettings.Items.Remove(selectedItem);
                }

                _userSettings.Save();
            });

            AddJobCommand = new RelayCommand(p =>
            {
                var selectedItem = p as Item;

                var dlg = _viewLocator.NewStatusDialog(selectedItem);

                dlg.ShowDialog();
            });

            RemoveJobCommand = RemoveFolderCommand;

            BuildCommand = new RelayCommand(p =>
            {
                var selectedItem = p as Item;

                if (selectedItem == null)
                {
                    return;
                }

                MessageBox.Show("Build " + selectedItem.Name);
            });
        }

        #region Commands

        public ICommand BuildCommand { get; set; }
        public ICommand AddFolderCommand { get; set; }
        public ICommand RemoveFolderCommand { get; set; }
        public ICommand AddJobCommand { get; set; }
        public ICommand RemoveJobCommand { get; set; }

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


