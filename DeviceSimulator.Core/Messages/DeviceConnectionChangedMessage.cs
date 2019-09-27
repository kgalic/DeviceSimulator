using MvvmCross.Plugin.Messenger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessagePublisher.Core
{
    public class DeviceConnectionChangedMessage : MvxMessage
    {
        private bool _isConnected;
        public DeviceConnectionChangedMessage(object sender, bool isConnected) : base(sender)
        {
            _isConnected = isConnected;
        }

        public bool IsConnected => _isConnected;
    }
}
