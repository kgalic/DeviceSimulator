using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.Plugin.Messenger;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DeviceSimulator.Core
{
    public class EventGridViewModel : BasePublisherViewModel
    {
        #region Fields

        private MvxSubscriptionToken _deviceStatusChangedMessageToken;

        private EventGridSetting _eventGridSetting;

        #endregion

        #region Constructors & Lifecycle

        public EventGridViewModel(IEventGridService eventGridService)
        {
            _publisherService = eventGridService;

            _eventGridSetting = new EventGridSetting();

            SetConnectionStatus();

            _deviceStatusChangedMessageToken = _messageService.Subscribe<EventGridStatusUpdatedMessage>(HandleDeviceStatus);
        }

        #endregion

        #region Public

        public string Endpoint
        {
            get
            {
                return _eventGridSetting.Endpoint;
            }
            set
            {
                _eventGridSetting.Endpoint = value;
                RaisePropertyChanged(() => Endpoint);
            }
        }

        public override string MessagePayload
        {
            get
            {
                return _eventGridSetting.Data;
            }
            set
            {
                _eventGridSetting.Data = value;
                RaisePropertyChanged(() => MessagePayload);
            }
        }

        public string Key
        {
            get
            {
                return _eventGridSetting.Key;
            }
            set
            {
                _eventGridSetting.Key = value;
                RaisePropertyChanged(() => Key);
            }
        }

        public string TopicName
        {
            get
            {
                return _eventGridSetting.Topic;
            }
            set
            {
                _eventGridSetting.Topic = value;
                RaisePropertyChanged(() => TopicName);
            }
        }

        public string Subject
        {
            get
            {
                return _eventGridSetting.Subject;
            }
            set
            {
                _eventGridSetting.Subject = value;
                RaisePropertyChanged(() => Subject);
            }
        }

        public string DataVersion
        {
            get
            {
                return _eventGridSetting.DataVersion;
            }
            set
            {
                _eventGridSetting.DataVersion = value;
                RaisePropertyChanged(() => DataVersion);
            }
        }

        public string EventType
        {
            get
            {
                return _eventGridSetting.EventType;
            }
            set
            {
                _eventGridSetting.EventType = value;
                RaisePropertyChanged(() => EventType);
            }
        }

        #endregion

        #region Translations

        public string EndpointPlaceholder
        {
            get => _translationsService.GetString("EndpointPlaceholder");
        }

        public string KeyPlaceholder
        {
            get => _translationsService.GetString("KeyPlaceholder");
        }

        public string TopicNamePlaceholder
        {
            get => _translationsService.GetString("TopicNamePlaceholder");
        }

        public string SubjectPlaceholder
        {
            get => _translationsService.GetString("SubjectPlaceholder");
        }

        public string DataVersionPlaceholder
        {
            get => _translationsService.GetString("DataVersionPlaceholder");
        }

        public string EventTypePlaceholder
        {
            get => _translationsService.GetString("EventTypePlaceholder");
        }

        #endregion

        #region Commands

        public override IMvxCommand ConnectCommand
        {
            get
            {
                return new MvxCommand(async () =>
                {
                    if (EventGridServiceInstance.IsConnected)
                    {
                        await EventGridServiceInstance.Disconnect().ConfigureAwait(false); ;
                    }
                    else
                    {
                        await EventGridServiceInstance.Connect(endpoint: Endpoint,
                                                       key: Key,
                                                       topic: TopicName,
                                                       subject: Subject,
                                                       eventType: EventType,
                                                       dataVersion: DataVersion).ConfigureAwait(false);
                    }
                    SetConnectionStatus();
                });
            }
        }

        public override IMvxCommand SaveSettingsCommand
        {
            get
            {
                return new MvxCommand(async () =>
                {
                    await _filePickerService.SaveDeviceSettingFromDiskAsync(_eventGridSetting);
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
                        var eventGridSettingString = await _filePickerService.LoadSettingsFromDiskAsync();
                        var eventGridSetting = JsonConvert.DeserializeObject<EventGridSetting>(eventGridSettingString);
                        if (eventGridSetting != null)
                        {
                            await ResetAll();
                            _eventGridSetting = eventGridSetting;
                            MessagePayload = _eventGridSetting.Data;
                            Endpoint = _eventGridSetting.Endpoint;
                            Key = _eventGridSetting.Key;
                            TopicName = _eventGridSetting.Topic;
                            EventType = _eventGridSetting.EventType;
                            Subject = _eventGridSetting.Subject;
                            DataVersion = _eventGridSetting.DataVersion;
                        }
                    }
                    catch
                    {
                        var exceptionMessage = _translationsService.GetString("ErrorLoadingFileMessageException");
                        _consoleLoggerService.LogEventGrid(exceptionMessage);
                    }
                });
            }
        }

        #endregion

        #region Private

        private IEventGridService EventGridServiceInstance => _publisherService as IEventGridService;

        #endregion

        #region Private Methods

        private void HandleDeviceStatus(EventGridStatusUpdatedMessage message)
        {
            if (!string.IsNullOrEmpty(message.Status))
            {
                OutputLog += $"{message.Status}\n";
            }
        }

        private void SetConnectionStatus()
        {
            ConnectionStatus = EventGridServiceInstance.IsConnected ? _translationsService.GetString("Disconnect")
                                                                    : _translationsService.GetString("Connect");
        }

        private void StartTimer()
        {
            _isTimerOn = true;
            _messageService.Publish(new StartTimerServiceMessage(this));
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

            if (EventGridServiceInstance.IsConnected)
            {
                await EventGridServiceInstance.Disconnect().ConfigureAwait(false); ;
            }

            SetConnectionStatus();

            OutputLog = string.Empty;
        }

        #endregion
    }
}
