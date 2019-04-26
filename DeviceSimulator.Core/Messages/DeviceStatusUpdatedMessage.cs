using MvvmCross.Plugin.Messenger;using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceSimulator.Core
{
    public class DeviceStatusUpdatedMessage : MvxMessage
    {
        public string Status { get; private set; }
        public DeviceStatusUpdatedMessage(object sender, string status) : base(sender)
        {
            Status = status;
        }
    }
}
