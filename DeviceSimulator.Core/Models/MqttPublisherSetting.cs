using System;
using System.Collections.Generic;
using System.Text;

namespace MessagePublisher.Core
{
    public class MqttPublisherSetting : BaseSetting
    {
        public string HostName { get; set; }

        public string Topic { get; set; }

        public string Message { get; set; }
    }
}
