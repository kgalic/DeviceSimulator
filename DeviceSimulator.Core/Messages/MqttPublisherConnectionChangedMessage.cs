using MvvmCross.Plugin.Messenger;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessagePublisher.Core
{
    public class MqttPublisherConnectionChangedMessage : MvxMessage
    {
        private bool _isConnected;
        public MqttPublisherConnectionChangedMessage(object sender, bool isConnected) : base(sender)
        {
            _isConnected = isConnected;
        }

        public bool IsConnected => _isConnected;
    }
}
