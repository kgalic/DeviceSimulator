using System;
using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.Plugin.Messenger;
using Newtonsoft.Json;

namespace MessagePublisher.Core
{
    public class EventHubViewModel : BasePublisherViewModel
    {
        #region Fields

        protected readonly IDeviceSettingDataService _deviceSettingDataService;
        protected readonly ITimerService<EventHubViewModel> _timerService;

        private MvxSubscriptionToken _statusChangedMessageToken;

        private EventHubSetting _eventHubSetting;

        #endregion

        #region Constructors & Lifecycle

        public EventHubViewModel(IEventHubService eventHubService)
        {
            _publisherService = eventHubService;
            _deviceSettingDataService = Mvx.IoCProvider.Resolve<IDeviceSettingDataService>();
            _timerService = Mvx.IoCProvider.Resolve<ITimerService<EventHubViewModel>>();

            _eventHubSetting = new EventHubSetting();

            SetConnectionStatus();

            ConsoleLogType = ConsoleLogTypes.EventHub;

            _statusChangedMessageToken = _messageService.Subscribe<EventHubStatusUpdatedMessage>(HandleStatusChangedMessage);
        }

        #endregion

        #region Public
        
        public string ConnectionString
        {
            get
            {
                return _eventHubSetting.ConnectionString;
            }
            set
            {
                _eventHubSetting.ConnectionString = value;
                RaisePropertyChanged(() => ConnectionString);
            }
        }

        public override string MessagePayload
        {
            get
            {
                return _eventHubSetting.Message;
            }
            set
            {
                _eventHubSetting.Message = value;
                RaisePropertyChanged(() => MessagePayload);
            }
        }
        public override bool IsRunning => _timerService.IsRunning;

        #endregion

        #region Commands

        public override IMvxCommand ConnectCommand
        {
            get
            {
                return new MvxCommand(async () =>
                {
                    try
                    {
                        if (EventHubService.IsConnected)
                        {
                            await EventHubService.Disconnect().ConfigureAwait(false); ;
                        }
                        else
                        {
                            await EventHubService.Connect(ConnectionString).ConfigureAwait(false);
                        }
                        SetConnectionStatus();
                    }
                    catch (Exception e)
                    {
                        var exceptionMessage = string.Format(_translationsService.GetString("EventHubNotReachableMessageException"),
                                                                Environment.NewLine,
                                                                e.Message);

                        _consoleLoggerService.Log(value: exceptionMessage,
                                                  logType: ConsoleLogTypes.EventHub);
                    }
                });
            }
        }

        public override IMvxCommand SaveSettingsCommand
        {
            get
            {
                return new MvxCommand(async () =>
                {
                    try
                    {
                        await _filePickerService.SaveDeviceSettingFromDiskAsync(_eventHubSetting);
                    }
                    catch
                    {
                        _consoleLoggerService.Log(value: _translationsService.GetString("SaveFileException"), 
                                                  logType: ConsoleLogType);
                    }
                });
            }
        }

        public override IMvxCommand LoadSettingsCommand
        {
            get
            {
                return new MvxCommand(async () =>
                {
                    try
                    {
                        var deviceSettingsString = await _filePickerService.LoadSettingsFromDiskAsync();
                        var deviceSettings = JsonConvert.DeserializeObject<EventHubSetting>(deviceSettingsString);
                        if (deviceSettings != null)
                        {
                            await ResetAll();
                            _eventHubSetting = deviceSettings;
                            MessagePayload = _eventHubSetting.Message;
                            ConnectionString = _eventHubSetting.ConnectionString;
                        }
                    }
                    catch
                    {
                        var exceptionMessage = _translationsService.GetString("ErrorLoadingFileMessageException");
                        _consoleLoggerService.Log(value: exceptionMessage,
                                                  logType: ConsoleLogType);
                    }
                });
            }
        }

        #endregion

        #region Translations

        public string ConnectionStringPlaceholder
        {
            get => _translationsService.GetString("EventHubConnectionString");
        }

        #endregion

        #region Private

        private IEventHubService EventHubService => _publisherService as IEventHubService;

        #endregion

        #region Protected Methods

        protected override void StartTimer()
        {
            _isTimerOn = true;
            _messageService.Publish(new StartTimerServiceMessage<EventHubViewModel>(this, DelayInMiliseconds));
            TimerStatusTitle = _translationsService.GetString("StopTimer");
        }

        protected override void StopTimer()
        {
            _isTimerOn = false;
            _messageService.Publish(new StopTimerServiceMessage<EventHubViewModel>(this));
            TimerStatusTitle = _translationsService.GetString("StartTimer");
        }

        #endregion
        #region Private Methods

        private void HandleStatusChangedMessage(EventHubStatusUpdatedMessage message)
        {
            if (!string.IsNullOrEmpty(message.Status))
            {
                OutputLog += $"{message.Status}\n";
            }
        }

        #endregion
    }
}
