using MvvmCross.Core.ViewModels;
using MvvmCross.Plugins.Messenger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceSimulator.Core
{
    public class MainViewModel : MvxViewModel
    {
        private readonly IDeviceService _deviceService;

        private MvxSubscriptionToken _deviceStatusChangedMessageToken;
        private MvxSubscriptionToken _deviceConnectionStatusChangedMessageToken;

        private string _deviceStatus;
        private string _message;
        private string _iotHubConnectionString;
        private string _deviceConnectionStatus;

        public MainViewModel(IDeviceService deviceService, IMvxMessenger messageService)
        {
            _deviceService = deviceService;

            DeviceStatus = _deviceService.Status ?? string.Empty;
            SetDeviceConnectionStatusForStatus(_deviceService.IsConnected);

            _deviceStatusChangedMessageToken = messageService.Subscribe<DeviceStatusUpdatedMessage>(HandleDeviceStatus);
            _deviceConnectionStatusChangedMessageToken = messageService.Subscribe<DeviceConnectionChangedMessage>(HandleDeviceConnectionStatus);
        }

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
                return _iotHubConnectionString;
            }
            set
            {
                _iotHubConnectionString = value;
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

        public IMvxCommand ConnectToIoTHubCommand
        {
            get
            {
                return new MvxCommand(() =>
                {
                    if (_deviceService.IsConnected)
                    {
                        _deviceService.DisconnectFromDevice();
                    }
                    else
                    {
                        _deviceService.ConnectToDevice(IoTHubConnectionString);
                    }
                });
            }
        }

        public IMvxCommand SendMessageCommand
        {
            get
            {
                return new MvxCommand(async () =>
                {
                    if (MessagePayload != null && MessagePayload.Trim().Count() > 0)
                    {
                        await _deviceService.SendRequest(MessagePayload);
                    }
                });
            }
        }

        private void HandleDeviceStatus(DeviceStatusUpdatedMessage message)
        {
            if (!string.IsNullOrEmpty(message.Status))
            {
                DeviceStatus += $"{message.Status}\n";
            }
        }

        private void HandleDeviceConnectionStatus(DeviceConnectionChangedMessage message)
        {
            SetDeviceConnectionStatusForStatus(message.IsConnected);
        }

        private void SetDeviceConnectionStatusForStatus(bool isConnected)
        {
            DeviceConnectionStatus = isConnected ? "Disconnect" : "Connect";
        }
    }
}
