using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DeviceSimulator.Core
{ 
    public interface IEventGridService : IPublisherService
    {
        Task Connect(string endpoint,
                     string key,
                     string topic,
                     string subject,
                     string eventType,
                     string dataVersion);
    }
}
