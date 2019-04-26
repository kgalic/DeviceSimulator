using MvvmCross.Plugin.Messenger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceSimulator.Core
{
    public class TimerService : ITimerService
    {
        private readonly IMvxMessenger _messageService;

        private MvxSubscriptionToken _stopTimerMessageToken;
        private MvxSubscriptionToken _startTimerMessageToken;

        private int _intervalInMiliseconds = 1000;

        private bool _isRunning;

        public TimerService(IMvxMessenger messageService)
        {
            _messageService = messageService;

            _stopTimerMessageToken = messageService.Subscribe<StopTimerServiceMessage>(StopTimer);
            _startTimerMessageToken = messageService.Subscribe<StartTimerServiceMessage>(StartTimer);
        }

        public bool IsRunning => _isRunning;

        public void Initialize()
        {
            if (!_isRunning)
            {
                _isRunning = true;
                RunTimer();
            }
        }

        private void StopTimer(MvxMessage message)
        {
            _isRunning = false;
        }

        private void StartTimer(StartTimerServiceMessage message)
        {
            if (message.IntervalInMiliseconds != 0)
            {
                _intervalInMiliseconds = message.IntervalInMiliseconds;
            }

            Initialize();
        }

        private async void RunTimer()
        {
            while (_isRunning)
            {
                await Task.Delay(_intervalInMiliseconds);
                _messageService.Publish(new TimerServiceTriggeredMessage(this));
            }
        }
    }
}
