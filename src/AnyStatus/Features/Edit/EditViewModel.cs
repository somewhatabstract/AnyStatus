﻿using AnyStatus.Interfaces;
using AnyStatus.Models;
using System;
using System.Windows.Input;

namespace AnyStatus.Features.Edit
{
    public class EditViewModel
    {
        private Item _item;
        private Item _sourceItem;

        private readonly ILogger _logger;
        private readonly IUserSettings _userSettings;
        private readonly IJobScheduler _jobScheduler;

        public event EventHandler CloseRequested;

        public EditViewModel(IUserSettings userSettings, IJobScheduler jobScheduler, ILogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));
            if (userSettings == null)
                throw new ArgumentNullException(nameof(userSettings));
            if (jobScheduler == null)
                throw new ArgumentNullException(nameof(jobScheduler));

            _logger = logger;
            _jobScheduler = jobScheduler;
            _userSettings = userSettings;

            Initialize();
        }

        private void Initialize()
        {
            SaveCommand = new RelayCommand(p =>
            {
                try
                {
                    //todo: add validation

                    _sourceItem.ReplaceWith(_item);

                    _userSettings.Save();

                    _jobScheduler.Reschedule(_item);
                }
                catch (Exception ex)
                {
                    _logger.Info("Failed to save changes. Exception:" + ex.ToString());
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

        public Item Item
        {
            get
            {
                return _item;
            }
            set
            {
                _item = (Item)value.Clone();
                _sourceItem = value;
            }
        }

        #region Commands

        public ICommand SaveCommand { get; set; }

        public ICommand CancelCommand { get; set; }

        #endregion
    }
}
