using MvvmCross.Plugin.Messenger;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceSimulator.Core
{
    public class ServiceBusStatusUpdatedMessage : MvxMessage
    {
        public string Status { get; private set; }

        public ServiceBusStatusUpdatedMessage(object sender, string status) : base(sender)
        {
            Status = status;
        }
    }
}
