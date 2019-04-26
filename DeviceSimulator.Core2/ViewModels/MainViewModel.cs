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
    public class MainViewModel : MvxViewModel
    {
        private const string StopTimerTitle = "Stop Timer";
        private const string StartTimerTitle = "Start Timer";

        public const int SliderMinimum = 1;
        public const int SliderMaximum = 10;

        private readonly IDeviceService _deviceService;
        private readonly ITimerService _timerService;
        private readonly IMvxMessenger _messageService;

        private MvxSubscriptionToken _deviceStatusChangedMessageToken;
        private MvxSubscriptionToken _deviceConnectionStatusChangedMessageToken;
        private MvxSubscriptionToken _timerServiceTriggeredMessageToken;

        private string _deviceStatus;
        private string _message;
        private string _iotHubConnectionString;
        private string _deviceConnectionStatus;
        private string _timerStatusTitle;

        private int _delayInSeconds;

        private bool _isDelayRangeVisible;

        public MainViewModel(IDeviceService deviceService, 
            ITimerService timerService,
            IMvxMessenger messageService)
        {
            _deviceService = deviceService;
            _timerService = timerService;
            _messageService = messageService;

            TimerStatusTitle = StartTimerTitle;
            _delayInSeconds = SliderMinimum;

            DeviceStatus = _deviceService.Status ?? string.Empty;
            SetDeviceConnectionStatusForStatus(_deviceService.IsConnected);

            _deviceStatusChangedMessageToken = messageService.Subscribe<DeviceStatusUpdatedMessage>(HandleDeviceStatus);
            _deviceConnectionStatusChangedMessageToken = messageService.Subscribe<DeviceConnectionChangedMessage>(HandleDeviceConnectionStatus);
            _timerServiceTriggeredMessageToken = messageService.Subscribe<TimerServiceTriggeredMessage>(HandleTimerTrigger);
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

        public bool IsDelayRangeVisible
        {
            get
            {
                return _isDelayRangeVisible;
            }
            set
            {
                _isDelayRangeVisible = value;
                RaisePropertyChanged(() => IsDelayRangeVisible);
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
                    await SendMessagePayload();
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
                        TimerStatusTitle = StartTimerTitle;
                        IsDelayRangeVisible = false;
                    }
                    else
                    {
                        _messageService.Publish(new StartTimerServiceMessage(this));
                        TimerStatusTitle = StopTimerTitle;
                        IsDelayRangeVisible = true;
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

        private void HandleTimerTrigger(MvxMessage message)
        {
            SendMessagePayload();
        }

        private async Task SendMessagePayload()
        {
            if (MessagePayload != null && MessagePayload.Trim().Count() > 0)
            {
                await _deviceService.SendRequest(MessagePayload);
            }
        }
    }
}
