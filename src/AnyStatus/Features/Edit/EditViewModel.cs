using AnyStatus.Infrastructure;
using AnyStatus.Interfaces;
using AnyStatus.Models;
using FluentScheduler;
using System;
using System.Diagnostics;
using System.Windows.Input;

namespace AnyStatus.Features.Edit
{
    public class EditViewModel
    {
        private IUserSettings _userSettings;

        public event EventHandler CloseRequested;

        public EditViewModel(IUserSettings userSettings)
        {
            if (userSettings == null)
                throw new ArgumentNullException(nameof(userSettings));

            _userSettings = userSettings;

            Initialize();
        }

        private void Initialize()
        {
            SaveCommand = new RelayCommand(p =>
            {
                try
                {
                    _userSettings.Save();

                    //todo: this is needed only if the trigger has changed (?)
                    JobManager.RemoveJob(Item.Id.ToString());
                    var job = TinyIoCContainer.Current.Resolve<ScheduledJob>();
                    job.Item = Item;
                    JobManager.AddJob(job,
                        s => s.WithName(Item.Id.ToString()).ToRunNow().AndEvery(Item.Interval).Minutes());
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
                finally
                {
                    CloseRequested?.Invoke(this, EventArgs.Empty);
                }
            });

            CancelCommand = new RelayCommand(p =>
            {
                CloseRequested?.Invoke(this, EventArgs.Empty);
            });
        }

        public Item Item { get; set; }

        #region Commands

        public ICommand SaveCommand { get; set; }

        public ICommand CancelCommand { get; set; }

        #endregion
    }
}
