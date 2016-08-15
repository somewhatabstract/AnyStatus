using AnyStatus.Interfaces;
using AnyStatus.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace AnyStatus.ViewModels
{
    public class NewStatusViewModel : INotifyPropertyChanged
    {
        private Template _selectedTemplate;
        private IUserSettings _userSettings;

        public event EventHandler CloseRequested;

        public NewStatusViewModel(IUserSettings userSettings)
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
            Templates = new List<Template> {
                new Template
                {
                    Name = "AnyStatus Job",
                    Item = new Job()
                }
            };

            AddCommand = new RelayCommand(p =>
            {
                var item = SelectedTemplate.Item;

                if (Parent != null)
                {
                    item.Parent = Parent;
                    Parent.Items.Add(item);
                    Parent.IsExpanded = true;
                }
                else
                {
                    _userSettings.Items.Add(item);
                }

                _userSettings.Save();

                CloseRequested?.Invoke(this, EventArgs.Empty);
            });

            CancelCommand = new RelayCommand(p =>
            {
                CloseRequested?.Invoke(this, EventArgs.Empty);
            });
        }

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

        public List<Template> Templates { get; set; }

        public Item Parent { get; internal set; }

        #region Command
        
        public ICommand AddCommand { get; set; }

        public ICommand TestCommand { get; set; }

        public ICommand CancelCommand { get; set; }

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