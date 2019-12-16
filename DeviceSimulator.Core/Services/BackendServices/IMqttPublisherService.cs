using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MessagePublisher.Core
{
    public interface IMqttPublisherService : IPublisherService
    {
        Task Connect(string hostName, string topic);
    }
}
