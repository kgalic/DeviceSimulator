using MvvmCross.Plugin.Messenger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceSimulator.Core
{
    public class StopTimerServiceMessage : MvxMessage
    {
        public StopTimerServiceMessage(object sender) : base(sender) { }
    }
}
