using MvvmCross.Plugin.Messenger;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessagePublisher.Core
{
    public class MqttPublisherStatusUpdatedMessage : MvxMessage
    {
        public string Status { get; private set; }
        public MqttPublisherStatusUpdatedMessage(object sender, string status) : base(sender)
        {
            Status = status;
        }
    }
}
