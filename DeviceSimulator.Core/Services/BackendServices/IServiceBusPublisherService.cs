using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MessagePublisher.Core
{
    public interface IServiceBusPublisherService : IPublisherService
    {
        Task Connect(string connectionString, 
                     string entity,
                     ServiceBusPublisherTypes serviceBusPublisherType);
    }

    public enum ServiceBusPublisherTypes
    {
        Queue = 0,
        Topic
    }
}
