using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceSimulator.Core
{
    public class EventGridSetting : BaseSetting
    {
        public string Endpoint { get; set; }

        public string Key { get; set; }

        public string Topic { get; set; }

        public string Subject { get; set; }

        public string EventType { get; set; }

        public string DataVersion { get; set; }

        public string Data { get; set; }
    }
}
