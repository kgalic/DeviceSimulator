using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DeviceSimulator.Core
{
    public interface IPublisherService
    {
        Task Connect(string connectionString);

        Task Disconnect();

        Task SendRequest(string Request);

        bool IsConnected { get; }
    }
}
