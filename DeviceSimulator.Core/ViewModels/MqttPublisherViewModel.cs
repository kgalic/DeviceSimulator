using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.Plugin.Messenger;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessagePublisher.Core
{
    public class MqttPublisherViewModel : BasePublisherViewModel
    {
        #region Fields

        protected readonly IDeviceSettingDataService _deviceSettingDataService;
        protected readonly ITimerService<MqttPublisherViewModel> _timerService;

        private MvxSubscriptionToken _statusChangedMessageToken;

        private MqttPublisherSetting _mqttPublisherSetting;

        #endregion

        #region Constructors & Lifecycle 

        public MqttPublisherViewModel(IMqttPublisherService mqttPublisherService)
        {
            _publisherService = mqttPublisherService;
            _deviceSettingDataService = Mvx.IoCProvider.Resolve<IDeviceSettingDataService>();
            _timerService = Mvx.IoCProvider.Resolve<ITimerService<MqttPublisherViewModel>>();

            _mqttPublisherSetting = new MqttPublisherSetting();

            SetConnectionStatus();

            ConsoleLogType = ConsoleLogTypes.MqttPublisher;

            _statusChangedMessageToken = _messageService.Subscribe<MqttPublisherStatusUpdatedMessage>(HandleStatusChangedMessage);
            _timerServiceTriggeredMessageToken = _messageService.Subscribe<TimerServiceTriggeredMessage<MqttPublisherViewModel>>(HandleTimerTrigger);
        }

        #endregion

        #region Public

        public string HostName
        {
            get => _mqttPublisherSetting.HostName;
            set
            {
                _mqttPublisherSetting.HostName = value;
                RaisePropertyChanged();
            }
        }

        public string Topic
        {
            get => _mqttPublisherSetting.Topic;
            set
            {
                _mqttPublisherSetting.Topic = value;
                RaisePropertyChanged();
            }
        }

        public override string MessagePayload
        {
            get
            {
                return _mqttPublisherSetting.Message;
            }
            set
            {
                _mqttPublisherSetting.Message = value;
                RaisePropertyChanged();
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
                        if (MqttPublisherService.IsConnected)
                        {
                            await MqttPublisherService.Disconnect().ConfigureAwait(false); ;
                        }
                        else
                        {
                            await MqttPublisherService.Connect(HostName, Topic).ConfigureAwait(false);
                        }
                        SetConnectionStatus();
                    }
                    catch (Exception e)
                    {
                        var exceptionMessage = string.Format(_translationsService.GetString("MqttNotReachable"),
                                                                Environment.NewLine,
                                                                e.Message);

                        _consoleLoggerService.Log(value: exceptionMessage,
                                                  logType: ConsoleLogType);
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
                        await _filePickerService.SaveDeviceSettingFromDiskAsync(_mqttPublisherSetting);
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
                        var deviceSettings = JsonConvert.DeserializeObject<MqttPublisherSetting>(deviceSettingsString);
                        if (deviceSettings != null)
                        {
                            await ResetAll();
                            _mqttPublisherSetting = deviceSettings;
                            MessagePayload = _mqttPublisherSetting.Message;
                            Topic = _mqttPublisherSetting.Topic;
                            HostName = _mqttPublisherSetting.HostName;
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

        #region Protected Methods

        protected override void StartTimer()
        {
            _isTimerOn = true;
            _messageService.Publish(new StartTimerServiceMessage<MqttPublisherViewModel>(this, DelayInMiliseconds));
            TimerStatusTitle = _translationsService.GetString("StopTimer");
        }

        protected override void StopTimer()
        {
            _isTimerOn = false;
            _messageService.Publish(new StopTimerServiceMessage<MqttPublisherViewModel>(this));
            TimerStatusTitle = _translationsService.GetString("StartTimer");
        }

        #endregion

        #region Translations

        public string HostNamePlaceholder
        {
            get => _translationsService.GetString("MqttPublisherHostName");
        }

        public string TopicPlaceholder
        {
            get => _translationsService.GetString("MqttPublisherTopic");
        }

        #endregion

        #region Private

        private IMqttPublisherService MqttPublisherService => _publisherService as IMqttPublisherService;

        #endregion

        #region Private Methods

        private void HandleStatusChangedMessage(MqttPublisherStatusUpdatedMessage message)
        {
            if (!string.IsNullOrEmpty(message.Status))
            {
                OutputLog += $"{message.Status}\n";
            }
        }

        private void HandleTimerTrigger(TimerServiceTriggeredMessage<MqttPublisherViewModel> message)
        {
            _ = SendMessagePayload();
        }

        #endregion
    }
}
