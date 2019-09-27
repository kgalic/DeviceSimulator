using System;
using System.Collections.Generic;
using System.Text;

namespace MessagePublisher.Core
{
    public class ServiceBusSetting : BaseSetting
    {
        public string ConnectionString { get; set; }

        public string EntityName { get; set; }

        public string Message { get; set; }

        public ServiceBusPublisherTypes Type { get; set; }
    }
}
