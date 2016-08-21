using AnyStatus.Infrastructure;
using AnyStatus.Interfaces;
using AnyStatus.Models;
using FluentScheduler;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;

namespace AnyStatus.ViewModels
{
    public class NewItemViewModel : INotifyPropertyChanged
    {
        private Template _selectedTemplate;
        private IUserSettings _userSettings;

        public event EventHandler CloseRequested;

        public NewItemViewModel(IUserSettings userSettings)
        {
            if (userSettings == null)
                throw new ArgumentNullException(nameof(userSettings));

            _userSettings = userSettings;

            Templates = new List<Template> {
                new Template("Ping", new Ping()),
                new Template("TCP Port", new TcpPort()),
                new Template("HTTP Status", new HttpStatus()),
                new Template("Jenkins Build", new JenkinsBuild()),
                new Template("TeamCity Build", new TeamCityBuild()),
            };

            SelectedTemplate = Templates.First();

            Initialize();
        }

        private void Initialize()
        {
            AddCommand = new RelayCommand(p =>
            {
                var item = SelectedTemplate.Item;

                item.Id = Guid.NewGuid();

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

                JobManager.AddJob(new ScheduledJob(item), schedule => schedule.WithName(item.Id.ToString()).ToRunNow().AndEvery(item.Interval).Minutes());

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

        #region Commands

        public ICommand AddCommand { get; set; }

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