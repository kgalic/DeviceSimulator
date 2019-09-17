using MvvmCross.Plugin.Messenger;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceSimulator.Core.Messages
{
    public class DirectMethodStatusUpdatedMessage : MvxMessage
    {
        public string Status { get; private set; }
        public DirectMethodStatusUpdatedMessage(object sender, string status) : base(sender)
        {
            Status = status;
        }
    }
}
