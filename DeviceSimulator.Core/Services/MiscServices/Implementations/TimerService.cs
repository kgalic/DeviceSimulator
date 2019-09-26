using MvvmCross.Plugin.Messenger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceSimulator.Core
{
    public class TimerService<T> : ITimerService<T> where T : BasePublisherViewModel
    {
        private readonly IMvxMessenger _messageService;

        private MvxSubscriptionToken _stopTimerMessageToken;
        private MvxSubscriptionToken _startTimerMessageToken;

        private int _intervalInMiliseconds = 1000;

        private bool _isRunning;

        public TimerService(IMvxMessenger messageService)
        {
            _messageService = messageService;

            _stopTimerMessageToken = messageService.Subscribe<StopTimerServiceMessage<T>>(StopTimer);
            _startTimerMessageToken = messageService.Subscribe<StartTimerServiceMessage<T>>(StartTimer);
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

        private void StartTimer(StartTimerServiceMessage<T> message)
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
                _messageService.Publish(new TimerServiceTriggeredMessage<T>(this));
            }
        }
    }
}
