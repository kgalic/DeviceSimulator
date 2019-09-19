using MvvmCross.Commands;
using MvvmCross.Plugin.Messenger;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceSimulator.Core.ViewModels
{
    public class HomeViewModel : MvxViewModel
    {
        #region Fields

        public const int SliderMinimum = 1;
        public const int SliderMaximum = 10;

        private readonly IDeviceService _deviceService;
        private readonly ITimerService _timerService;
        private readonly IMvxMessenger _messageService;
        private readonly ITranslationsService _translationsService;
        private readonly IDeviceSettingDataService _deviceSettingDataService;

        private MvxSubscriptionToken _deviceStatusChangedMessageToken;
        private MvxSubscriptionToken _timerServiceTriggeredMessageToken;

        private DeviceSetting _deviceSetting;

        private string _deviceStatus;
        private string _message;
        private string _deviceConnectionStatus;
        private string _timerStatusTitle;

        private int _delayInSeconds;

        #endregion

        #region Constructors & Lifecycle

        public HomeViewModel(IDeviceService deviceService,
                             ITimerService timerService,
                             IMvxMessenger messageService,
                             ITranslationsService translationsService,
                             IDeviceSettingDataService deviceSettingDataService)
        {
            _deviceService = deviceService;
            _timerService = timerService;
            _messageService = messageService;
            _translationsService = translationsService;
            _deviceSettingDataService = deviceSettingDataService;

            TimerStatusTitle = _translationsService.GetString("StartTimer"); ;
            _delayInSeconds = SliderMinimum;

            DeviceStatus = string.Empty;
            SetDeviceConnectionStatusForStatus();

            _deviceStatusChangedMessageToken = messageService.Subscribe<DeviceStatusUpdatedMessage>(HandleDeviceStatus);
            _timerServiceTriggeredMessageToken = messageService.Subscribe<TimerServiceTriggeredMessage>(HandleTimerTrigger);
        }

        public override Task Initialize()
        {
            _deviceSetting = new DeviceSetting();
            _deviceSettingDataService.DeviceSetting = _deviceSetting;
            return base.Initialize();
        }

        #endregion

        #region Public

        public string DeviceStatus
        {
            get
            {
                return _deviceStatus;
            }
            private set
            {
                _deviceStatus = value;
                RaisePropertyChanged(() => DeviceStatus);
            }
        }

        public string IoTHubConnectionString
        {
            get
            {
                return _deviceSetting.ConnectionString;
            }
            set
            {
                _deviceSetting.ConnectionString = value;
                RaisePropertyChanged(() => IoTHubConnectionString);
            }
        }

        public string DeviceConnectionStatus
        {
            get
            {
                return _deviceConnectionStatus;
            }
            set
            {
                _deviceConnectionStatus = value;
                RaisePropertyChanged(() => DeviceConnectionStatus);
            }
        }

        public string MessagePayload
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
                RaisePropertyChanged(() => MessagePayload);
            }
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
                _messageService.Publish(new StartTimerServiceMessage(this, _delayInSeconds * 1000));
                RaisePropertyChanged(() => DelayInSeconds);
            }
        }

        #endregion

        #region Translations

        public string IoTHubConnectionStringPlaceholder
        {
            get => _translationsService.GetString("IoTHubConnectionString");
        }

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
                    if (_deviceService.IsConnected)
                    {
                        await _deviceService.DisconnectFromDevice().ConfigureAwait(false); ;
                    }
                    else
                    {
                        await _deviceService.ConnectToDevice(IoTHubConnectionString).ConfigureAwait(false);
                    }
                    SetDeviceConnectionStatusForStatus();
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

        public IMvxCommand SaveSettingsCommand
        {
            get
            {
                return new MvxCommand(() =>
                {
                });
            }
        }

        public IMvxCommand LoadSettingsCommand
        {
            get
            {
                return new MvxCommand(() =>
                {
                });
            }
        }

        public IMvxCommand StartTimerServiceCommand
        {
            get
            {
                return new MvxCommand(() =>
                {
                    if (_timerService.IsRunning)
                    {
                        _messageService.Publish(new StopTimerServiceMessage(this));
                        TimerStatusTitle = _translationsService.GetString("StartTimer");
                    }
                    else
                    {
                        _messageService.Publish(new StartTimerServiceMessage(this));
                        TimerStatusTitle = _translationsService.GetString("StopTimer");
                    }
                });
            }
        }

        #endregion

        #region Private Methods

        private void HandleDeviceStatus(DeviceStatusUpdatedMessage message)
        {
            if (!string.IsNullOrEmpty(message.Status))
            {
                DeviceStatus += $"{message.Status}\n";
            }
        }

        private void SetDeviceConnectionStatusForStatus()
        {
            DeviceConnectionStatus = _deviceService.IsConnected ? _translationsService.GetString("Disconnect") 
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
                    await _deviceService.SendRequest(MessagePayload);
                }
            }
            catch
            {
                Console.WriteLine(_translationsService.GetString("SendingD2CMessageException"));
            }
        }

        #endregion
    }
}

