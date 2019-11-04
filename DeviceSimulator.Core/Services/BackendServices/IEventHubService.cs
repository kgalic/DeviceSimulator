using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MessagePublisher.Core
{
    public interface IEventHubService : IPublisherService
    {
        Task Connect(string connectionString);
    }
}
