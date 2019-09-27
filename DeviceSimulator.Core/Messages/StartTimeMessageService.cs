using MvvmCross.Plugin.Messenger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessagePublisher.Core
{
    public class StartTimerServiceMessage<T> : MvxMessage where T : BasePublisherViewModel
    {
        private int _intervalInMiliseconds;

        public StartTimerServiceMessage(object sender, int intervalInMiliseconds = 0) : base(sender)
        {
            _intervalInMiliseconds = intervalInMiliseconds;
        }

        public int IntervalInMiliseconds => _intervalInMiliseconds;
    }
}
