using MvvmCross.Plugin.Messenger;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessagePublisher.Core
{
    public class EventHubStatusUpdatedMessage : MvxMessage
    {
        public string Status { get; private set; }
        public EventHubStatusUpdatedMessage(object sender, string status) : base(sender)
        {
            Status = status;
        }
    }
}
