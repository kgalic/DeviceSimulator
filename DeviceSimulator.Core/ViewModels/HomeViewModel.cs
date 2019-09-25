using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.Plugin.Messenger;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DeviceSimulator.Core.ViewModels
{
    public class HomeViewModel : BasePublisherViewModel
    {
        #region Fields

        protected readonly IDeviceSettingDataService _deviceSettingDataService;

        private MvxSubscriptionToken _deviceStatusChangedMessageToken;

        private DeviceSetting _deviceSetting;

        #endregion

        #region Constructors & Lifecycle
         
        public HomeViewModel(IDeviceService deviceService) 
        {
            _publisherService = deviceService;
            _deviceSettingDataService = Mvx.IoCProvider.Resolve<IDeviceSettingDataService>();

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

            _deviceStatusChangedMessageToken = _messageService.Subscribe<DeviceStatusUpdatedMessage>(HandleDeviceStatus);
        }

        public override void ViewDisappearing()
        {
            base.ViewDisappearing();
            _deviceSettingDataService.DeviceSetting = _deviceSetting;
        }

        #endregion

        #region Public

        public override string ConnectionString
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

        #endregion

        #region Translations

        public string IoTHubConnectionStringPlaceholder
        {
            get => _translationsService.GetString("IoTHubConnectionString");
        }

        #endregion

        #region Commands

        public IMvxCommand ConnectCommand
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
                        var deviceSettings =(DeviceSetting) await _filePickerService.LoadDeviceSettingFromDiskAsync();
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
                        _consoleLoggerService.Log(exceptionMessage);
                    }
                });
            }
        }

        #endregion

        #region Private

        private IDeviceService DeviceService => _publisherService as IDeviceService;

        #endregion

        #region Private Methods

        private void HandleDeviceStatus(DeviceStatusUpdatedMessage message)
        {
            if (!string.IsNullOrEmpty(message.Status))
            {
                OutputLog += $"{message.Status}\n";
            }
        }

        private void SetDeviceConnectionStatusForStatus()
        {
            ConnectionStatus = DeviceService.IsConnected ? _translationsService.GetString("Disconnect") 
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
                    await DeviceService.SendRequest(MessagePayload);
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

