using MvvmCross.Plugin.Messenger;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessagePublisher.Core
{
    public class EventHubConnectionChangedMessage : MvxMessage
    {
        private bool _isConnected;
        public EventHubConnectionChangedMessage(object sender, bool isConnected) : base(sender)
        {
            _isConnected = isConnected;
        }

        public bool IsConnected => _isConnected;
    }
}
