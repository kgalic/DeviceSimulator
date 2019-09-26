using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.Plugin.Messenger;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DeviceSimulator.Core.ViewModels
{
    public class HomeViewModel : BasePublisherViewModel
    {
        #region Fields

        protected readonly IDeviceSettingDataService _deviceSettingDataService;
        protected readonly ITimerService<HomeViewModel> _timerService;

        private MvxSubscriptionToken _deviceStatusChangedMessageToken;

        private DeviceSetting _deviceSetting;

        #endregion

        #region Constructors & Lifecycle
         
        public HomeViewModel(IDeviceService deviceService) 
        {
            _publisherService = deviceService;
            _deviceSettingDataService = Mvx.IoCProvider.Resolve<IDeviceSettingDataService>();
            _timerService = Mvx.IoCProvider.Resolve<ITimerService<HomeViewModel>>();

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

            ViewModelType = Types.D2CCommunication;

            _deviceStatusChangedMessageToken = _messageService.Subscribe<DeviceStatusUpdatedMessage>(HandleDeviceStatus);
            _timerServiceTriggeredMessageToken = _messageService.Subscribe<TimerServiceTriggeredMessage<HomeViewModel>>(HandleTimerTrigger);
        }

        public override void ViewDisappearing()
        {
            base.ViewDisappearing();
            _deviceSettingDataService.DeviceSetting = _deviceSetting;
        }

        #endregion

        #region Public

        public string ConnectionString
        {
            get
            {
                return _deviceSetting.ConnectionString;
            }
            set
            {
                _deviceSetting.ConnectionString = value;
                RaisePropertyChanged(() => ConnectionString);
            }
        }

        public override string MessagePayload
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

        public override bool IsRunning => _timerService.IsRunning;

        #endregion

        #region Translations

        public string IoTHubConnectionStringPlaceholder
        {
            get => _translationsService.GetString("IoTHubConnectionString");
        }

        #endregion

        #region Commands

        public override IMvxCommand ConnectCommand
        {
            get
            {
                return new MvxCommand(async () =>
                {
                    if (DeviceService.IsConnected)
                    {
                        await DeviceService.Disconnect().ConfigureAwait(false); ;
                    }
                    else
                    {
                        await DeviceService.Connect(ConnectionString).ConfigureAwait(false);
                    }
                    SetDeviceConnectionStatusForStatus();
                });
            }
        }

        public override IMvxCommand SaveSettingsCommand
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

        public override IMvxCommand LoadSettingsCommand
        {
            get
            {
                return new MvxCommand(async () =>
                {
                    try
                    {
                        var deviceSettingsString = await _filePickerService.LoadSettingsFromDiskAsync();
                        var deviceSettings = JsonConvert.DeserializeObject<DeviceSetting>(deviceSettingsString);
                        if (deviceSettings != null)
                        {
                            await ResetAll();
                            _deviceSetting = deviceSettings;
                            MessagePayload = _deviceSetting.Message;
                            ConnectionString = _deviceSetting.ConnectionString;
                            _deviceSettingDataService.DeviceSetting = _deviceSetting;
                        }
                    }
                    catch
                    {
                        var exceptionMessage = _translationsService.GetString("ErrorLoadingFileMessageException");
                        _consoleLoggerService.Log(value: exceptionMessage,
                                                  logType: ViewModelType);
                    }
                });
            }
        }

        #endregion

        #region Private

        private IDeviceService DeviceService => _publisherService as IDeviceService;

        #endregion

        #region ProtectedMethods

        protected override void StartTimer()
        {
            _isTimerOn = true;
            _messageService.Publish(new StartTimerServiceMessage<HomeViewModel>(this, DelayInMiliseconds));
            TimerStatusTitle = _translationsService.GetString("StopTimer");
        }

        protected override void StopTimer()
        {
            _isTimerOn = false;
            _messageService.Publish(new StopTimerServiceMessage<HomeViewModel>(this));
            TimerStatusTitle = _translationsService.GetString("StartTimer");
        }

        #endregion

        #region Private Methods

        private void HandleDeviceStatus(DeviceStatusUpdatedMessage message)
        {
            if (!string.IsNullOrEmpty(message.Status))
            {
                OutputLog += $"{message.Status}\n";
            }
        }

        protected void HandleTimerTrigger(TimerServiceTriggeredMessage<HomeViewModel> message)
        {
            SendMessagePayload();
        }

        private void SetDeviceConnectionStatusForStatus()
        {
            ConnectionStatus = DeviceService.IsConnected ? _translationsService.GetString("Disconnect") 
                                                                : _translationsService.GetString("Connect");
        }

        private async Task ResetAll()
        {
            StopTimer();

            if (DeviceService.IsConnected)
            {
                await DeviceService.Disconnect().ConfigureAwait(false); ;
            }

            SetDeviceConnectionStatusForStatus();

            OutputLog = string.Empty;
        }

        #endregion
    }
}

