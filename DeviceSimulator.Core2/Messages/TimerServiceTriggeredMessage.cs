using MvvmCross.Plugin.Messenger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceSimulator.Core
{
    public class TimerServiceTriggeredMessage : MvxMessage
    {
        public TimerServiceTriggeredMessage(object sender) : base(sender) { }
    }
}
