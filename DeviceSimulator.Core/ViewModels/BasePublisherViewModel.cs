using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.Plugin.Messenger;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceSimulator.Core
{
    public abstract class BasePublisherViewModel : MvxViewModel
    {
        #region Fields

        public const int SliderMinimum = 1;
        public const int SliderMaximum = 10;

        protected IPublisherService _publisherService;

        protected readonly ITimerService _timerService;
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
            _timerService = Mvx.IoCProvider.Resolve<ITimerService>();
            _translationsService = Mvx.IoCProvider.Resolve<ITranslationsService>();
            _filePickerService = Mvx.IoCProvider.Resolve<IFilePickerService>();
            _consoleLoggerService = Mvx.IoCProvider.Resolve<IConsoleLoggerService>();
            _messageService = Mvx.IoCProvider.Resolve<IMvxMessenger>();
           
            TimerStatusTitle = _translationsService.GetString("StartTimer"); ;
            _delayInSeconds = SliderMinimum;

            OutputLog = string.Empty;

            _timerServiceTriggeredMessageToken = _messageService.Subscribe<TimerServiceTriggeredMessage>(HandleTimerTrigger);
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

        public abstract string ConnectionString
        {
            get; set;
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
                    _messageService.Publish(new StartTimerServiceMessage(this, DelayInMiliseconds));
                }
                RaisePropertyChanged(() => DelayInSeconds);
            }
        }

        public int DelayInMiliseconds => DelayInSeconds * 1000;

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

        public IMvxCommand ConnectToIoTHubCommand
        {
            get
            {
                return new MvxCommand(async () =>
                {
                    if (_publisherService.IsConnected)
                    {
                        await _publisherService.Disconnect().ConfigureAwait(false); ;
                    }
                    else
                    {
                        await _publisherService.Connect(ConnectionString).ConfigureAwait(false);
                    }
                    SetConnectionStatus();
                });
            }
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
                    if (_timerService.IsRunning)
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

        #region Private Methods

        private void SetConnectionStatus()
        {
            ConnectionStatus = _publisherService.IsConnected ? _translationsService.GetString("Disconnect")
                                                                : _translationsService.GetString("Connect");
        }

        private void HandleTimerTrigger(MvxMessage message)
        {
            SendMessagePayload();
        }

        private async Task SendMessagePayload()
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
                Console.WriteLine(_translationsService.GetString("SendingD2CMessageException"));
            }
        }

        private void StartTimer()
        {
            _isTimerOn = true;
            _messageService.Publish(new StartTimerServiceMessage(this, DelayInMiliseconds));
            TimerStatusTitle = _translationsService.GetString("StopTimer");
        }

        private void StopTimer()
        {
            _isTimerOn = false;
            _messageService.Publish(new StopTimerServiceMessage(this));
            TimerStatusTitle = _translationsService.GetString("StartTimer");
        }

        private async Task ResetAll()
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
