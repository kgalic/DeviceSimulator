﻿using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.Plugin.Messenger;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessagePublisher.Core
{
    public abstract class BasePublisherViewModel : MvxViewModel
    {
        #region Fields

        public const int SliderMinimum = 1;
        public const int SliderMaximum = 10;

        protected IPublisherService _publisherService;

        protected readonly IMvxMessenger _messageService;
        protected readonly ITranslationsService _translationsService;
        protected readonly IFilePickerService _filePickerService;
        protected readonly IConsoleLoggerService _consoleLoggerService;

        protected MvxSubscriptionToken _timerServiceTriggeredMessageToken;

        protected BaseSetting _setting;

        protected string _outputLog;
        protected string _connectionStatus;
        protected string _timerStatusTitle;

        protected int _delayInSeconds;
        protected bool _isTimerOn;

        #endregion

        #region Constructors & Lifecycle

        public BasePublisherViewModel()
        {
            _translationsService = Mvx.IoCProvider.Resolve<ITranslationsService>();
            _consoleLoggerService = Mvx.IoCProvider.Resolve<IConsoleLoggerService>();
            _messageService = Mvx.IoCProvider.Resolve<IMvxMessenger>();
            _filePickerService = Mvx.IoCProvider.Resolve<IFilePickerService>();

            TimerStatusTitle = _translationsService.GetString("StartTimer"); ;
            _delayInSeconds = SliderMinimum;

            OutputLog = string.Empty;

        }

        #endregion

        #region Public

        public string OutputLog
        {
            get
            {
                return _outputLog;
            }
            protected set
            {
                _outputLog = value;
                RaisePropertyChanged(() => OutputLog);
            }
        }

        public string ConnectionStatus
        {
            get
            {
                return _connectionStatus;
            }
            set
            {
                _connectionStatus = value;
                RaisePropertyChanged(() => ConnectionStatus);
            }
        }

        public abstract string MessagePayload
        {
            get; set;
        }

        public string TimerStatusTitle
        {
            get
            {
                return _timerStatusTitle;
            }
            set
            {
                _timerStatusTitle = value;
                RaisePropertyChanged(() => TimerStatusTitle);
            }
        }

        public int TimerDelayMinimumValue
        {
            get
            {
                return SliderMinimum;
            }
        }

        public int TimerDelayMaximumValue
        {
            get
            {
                return SliderMaximum;
            }
        }

        public int DelayInSeconds
        {
            get
            {
                return _delayInSeconds;
            }
            set
            {
                _delayInSeconds = value;
                if (_isTimerOn)
                {
                    StartTimer();
                }
                RaisePropertyChanged(() => DelayInSeconds);
            }
        }

        public int DelayInMiliseconds => DelayInSeconds * 1000;

        public ConsoleLogTypes ConsoleLogType
        {
            get;
            set;
        }

        public abstract bool IsRunning
        {
            get;
        }
            

        #endregion

        #region Translations

        public string MessagePayloadPlaceholder
        {
            get => _translationsService.GetString("MessagePayload");
        }

        public string SendMessageString
        {
            get => _translationsService.GetString("SendMessage");
        }

        public string SaveString
        {
            get => _translationsService.GetString("Save");
        }

        public string LoadString
        {
            get => _translationsService.GetString("Load");
        }

        #endregion

        #region Commands

        public abstract IMvxCommand ConnectCommand
        {
            get;
        }

        public IMvxCommand SendMessageCommand
        {
            get
            {
                return new MvxCommand(async () =>
                {
                    await SendMessagePayload();
                });
            }
        }

        public abstract IMvxCommand SaveSettingsCommand
        {
            get;
        }

        public abstract IMvxCommand LoadSettingsCommand
        {
            get;
        }

        public IMvxCommand StartTimerServiceCommand
        {
            get
            {
                return new MvxCommand(() =>
                {
                    if (IsRunning)
                    {
                        StopTimer();
                    }
                    else
                    {
                        StartTimer();
                    }
                });
            }
        }

        #endregion

        #region Protected Methods

        protected void SetConnectionStatus()
        {
            ConnectionStatus = _publisherService.IsConnected ? _translationsService.GetString("Disconnect")
                                                                : _translationsService.GetString("Connect");
        }

        protected async Task SendMessagePayload()
        {
            try
            {
                if (MessagePayload != null && MessagePayload.Trim().Count() > 0)
                {
                    await _publisherService.SendRequest(MessagePayload);
                }
            }
            catch
            {
                _consoleLoggerService.Log(value: _translationsService.GetString("SendingMessageException"),
                                          logType: ConsoleLogType);
            }
        }

        protected abstract void StartTimer();

        protected abstract void StopTimer();

        protected async Task ResetAll()
        {
            StopTimer();

            if (_publisherService.IsConnected)
            {
                await _publisherService.Disconnect().ConfigureAwait(false); ;
            }

            SetConnectionStatus();

            OutputLog = string.Empty;
        }

        #endregion
    }
}
