using AnyStatus.Interfaces;
using AnyStatus.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace AnyStatus.ViewModels
{
    public class OptionsViewModel : INotifyPropertyChanged
    {
        private Server _selectedServer;
        private IUserSettings _userSettings;

        public OptionsViewModel() : this(new UserSettings()) { }

        public OptionsViewModel(IUserSettings userSettings)
        {
            if (userSettings == null)
            {
                throw new ArgumentNullException(nameof(userSettings));
            }

            _userSettings = userSettings;

            Initialize();
        }

        private void Initialize()
        {
            //AddServerCommand = new RelayCommand(p =>
            //{
            //    var server = new Server
            //    {
            //        Name = "New Server",
            //        Url = "http://"
            //    };

            //    _userSettings.Servers.Add(server);

            //    _userSettings.Save();
            //});

            //RemoveServerCommand = new RelayCommand(p =>
            //{
            //    var result = MessageBox.Show("Are you sure?", "Reset Settings", MessageBoxButton.YesNo, MessageBoxImage.Asterisk);

            //    if (result != MessageBoxResult.Yes) return;

            //    var server = p as Server;

            //    if (server == null) return;

            //    _userSettings.Servers.Remove(server);

            //    _userSettings.Save();
            //});

            //ApplyCommand = new RelayCommand(p => _userSettings.Save());

            ResetCommand = new RelayCommand(p =>
            {
                var result = MessageBox.Show("Are you sure?", "Reset Settings", MessageBoxButton.YesNo, MessageBoxImage.Asterisk);

                if (result != MessageBoxResult.Yes) return;

                _userSettings.Reset();
            });
        }

        //public ObservableCollection<Server> Servers
        //{
        //    get
        //    {
        //        return _userSettings.Servers;
        //    }
        //}

        //public Server SelectedServer
        //{
        //    get
        //    {
        //        return _selectedServer;
        //    }
        //    set
        //    {
        //        _selectedServer = value;
        //        OnPropertyChanged();
        //    }
        //}

        //public ICommand ApplyCommand { get; set; }
        //public ICommand AddServerCommand { get; set; }
        //public ICommand RemoveServerCommand { get; set; }
        public ICommand ResetCommand { get; set; }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
