using MvvmCross.Plugin.Messenger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessagePublisher.Core
{
    public class TimerServiceTriggeredMessage<T> : MvxMessage where T : BasePublisherViewModel
    {
        public TimerServiceTriggeredMessage(object sender) : base(sender) { }
    }
}
