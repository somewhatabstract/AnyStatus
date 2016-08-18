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
            //todo: move to container/bootstrap

            Templates = new List<Template> {
                new Template
                {
                    Name = "Jenkins Build",
                    Item = new JenkinsBuild()
                },
                new Template
                {
                    Name = "HTTP Status",
                    Item = new HttpStatus()
                }
            };

            SelectedTemplate = Templates.FirstOrDefault();

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
                    try
                    {
                        var a = typeof(IHandler<>);
                        var b = a.MakeGenericType(item.GetType());
                        var handler = TinyIoCContainer.Current.Resolve(b);
                        b.GetMethod("Handle").Invoke(handler, new[] { item });
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                        item.Brush = Brushes.Gray;
                    }
                };

                JobManager.AddJob(job, schedule => schedule.NonReentrant().WithName(item.Id.ToString()).ToRunNow().AndEvery(item.Interval).Minutes());

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