using MvvmCross.Plugin.Messenger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceSimulator.Core
{
    public class StartTimerServiceMessage : MvxMessage
    {
        private int _intervalInMiliseconds;

        public StartTimerServiceMessage(object sender, int intervalInMiliseconds = 0) : base(sender)
        {
            _intervalInMiliseconds = intervalInMiliseconds;
        }

        public int IntervalInMiliseconds => _intervalInMiliseconds;
    }
}
