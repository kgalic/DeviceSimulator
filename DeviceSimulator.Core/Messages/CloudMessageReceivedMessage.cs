using MvvmCross.Plugin.Messenger;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceSimulator.Core
{
    public class CloudMessageReceivedMessage : MvxMessage
    {
        public string Message { get; private set; }

        public CloudMessageReceivedMessage(object sender, string message) : base(sender)
        {
            Message = message;
        }
    }
}
