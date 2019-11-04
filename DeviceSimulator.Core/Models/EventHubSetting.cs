using System;
using System.Collections.Generic;
using System.Text;

namespace MessagePublisher.Core
{
    public class EventHubSetting : BaseSetting
    {
        public string ConnectionString
        {
            get;
            set;
        }
        public string Message
        {
            get;
            set;
        }
    }
}
