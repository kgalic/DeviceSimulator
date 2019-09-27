using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MessagePublisher.Core
{
    public interface IPublisherService
    {
        Task Disconnect();

        Task SendRequest(string request);

        bool IsConnected { get; }
    }
}
