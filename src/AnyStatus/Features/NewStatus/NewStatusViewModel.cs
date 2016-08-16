using AnyStatus.Interfaces;
using AnyStatus.Models;
using FluentScheduler;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace AnyStatus.ViewModels
{
    public class NewStatusViewModel : INotifyPropertyChanged
    {
        private Template _selectedTemplate;
        private IUserSettings _userSettings;
        private ILogger _logger;

        public event EventHandler CloseRequested;

        public NewStatusViewModel(IUserSettings userSettings, ILogger logger)
        {
            if (userSettings == null)
            {
                throw new ArgumentNullException(nameof(userSettings));
            }

            _userSettings = userSettings;
            _logger = logger;

            Initialize();
        }

        private void Initialize()
        {
            //todo: move to container
            Templates = new List<Template> {
                new Template
                {
                    Name = "Jenkins Job",
                    Item = new JenkinsJob()
                },
                new Template
                {
                    Name = "HTTP Status",
                    Item = new HttpStatus()
                }
            };

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

                Action job = () =>
                {
                    Debug.WriteLine(DateTime.Now + " Running " + item.Name);
                    var a = typeof(IHandler<>);
                    var b = a.MakeGenericType(item.GetType());
                    var handler = TinyIoC.TinyIoCContainer.Current.Resolve(b);
                    b.GetMethod("Handle").Invoke(handler, new[] { item });
                };

                Action<Schedule> schedule = s => {
                    s.NonReentrant()
                     .WithName(item.Id.ToString())
                     .ToRunNow()
                     .AndEvery(5).Seconds();
                };

                JobManager.AddJob(job, schedule);
                
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