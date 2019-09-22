using MvvmCross.Commands;
using MvvmCross.Plugin.Messenger;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
        private readonly IFilePickerService _filePickerService;
        private readonly IConsoleLoggerService _consoleLoggerService;

        private MvxSubscriptionToken _deviceStatusChangedMessageToken;
        private MvxSubscriptionToken _timerServiceTriggeredMessageToken;

        private DeviceSetting _deviceSetting;

        private string _deviceStatus;
        private string _deviceConnectionStatus;
        private string _timerStatusTitle;

        private int _delayInSeconds;
        private bool _isTimerOn;

        #endregion

        #region Constructors & Lifecycle

        public HomeViewModel(IDeviceService deviceService,
                             ITimerService timerService,
                             ITranslationsService translationsService,
                             IDeviceSettingDataService deviceSettingDataService,
                             IFilePickerService filePickerService,
                             IConsoleLoggerService consoleLoggerService,
                             IMvxMessenger messageService)
        {
            _deviceService = deviceService;
            _timerService = timerService;
            _messageService = messageService;
            _translationsService = translationsService;
            _deviceSettingDataService = deviceSettingDataService;
            _filePickerService = filePickerService;
            _consoleLoggerService = consoleLoggerService;

            TimerStatusTitle = _translationsService.GetString("StartTimer"); ;
            _delayInSeconds = SliderMinimum;

            DeviceStatus = string.Empty;

            if (_deviceSettingDataService.DeviceSetting != null)
            {
                _deviceSetting = _deviceSettingDataService.DeviceSetting;
            }
            else
            {
                _deviceSetting = new DeviceSetting();
                _deviceSettingDataService.DeviceSetting = _deviceSetting;
            }
            SetDeviceConnectionStatusForStatus();

            _deviceStatusChangedMessageToken = messageService.Subscribe<DeviceStatusUpdatedMessage>(HandleDeviceStatus);
            _timerServiceTriggeredMessageToken = messageService.Subscribe<TimerServiceTriggeredMessage>(HandleTimerTrigger);
        }

        public override void ViewDisappearing()
        {
            base.ViewDisappearing();
            _deviceSettingDataService.DeviceSetting = _deviceSetting;
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
                return _deviceSetting.Message;
            }
            set
            {
                _deviceSetting.Message = value;
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
                if (_isTimerOn)
                {
                    _messageService.Publish(new StartTimerServiceMessage(this, _delayInSeconds * 1000));
                }
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
                return new MvxCommand(async () =>
                {
                    _deviceSetting.Message = MessagePayload;
                    await _filePickerService.SaveDeviceSettingFromDiskAsync(_deviceSetting);
                });
            }
        }

        public IMvxCommand LoadSettingsCommand
        {
            get
            {
                return new MvxCommand(async () =>
                {
                    try
                    {
                        var deviceSettings = await _filePickerService.LoadDeviceSettingFromDiskAsync();
                        if (deviceSettings != null)
                        {
                            await ResetAll();
                            _deviceSetting = deviceSettings;
                            MessagePayload = _deviceSetting.Message;
                            IoTHubConnectionString = _deviceSetting.ConnectionString;
                            _deviceSettingDataService.DeviceSetting = _deviceSetting;
                        }
                    }
                    catch
                    {
                        var exceptionMessage = _translationsService.GetString("ErrorLoadingFileMessageException");
                        _consoleLoggerService.Log(exceptionMessage);
                    }
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

            if (_deviceService.IsConnected)
            {
                await _deviceService.DisconnectFromDevice().ConfigureAwait(false); ;
            }

            SetDeviceConnectionStatusForStatus();

            DeviceStatus = string.Empty;
        }

        #endregion
    }
}

