using MvvmCross.Plugin.Messenger;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceSimulator.Core
{
    public class EventGridStatusUpdatedMessage : MvxMessage
    {
        public string Status { get; private set; }
        public EventGridStatusUpdatedMessage(object sender, string status) : base(sender)
        {
            Status = status;
        }
    }
}
