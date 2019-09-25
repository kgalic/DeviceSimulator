using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DeviceSimulator.Core
{
    public class EventGridService : IEventGridService
    {
        public bool IsConnected => throw new NotImplementedException();

        public Task Connect(string connectionString)
        {
            throw new NotImplementedException();
        }

        public Task Disconnect()
        {
            throw new NotImplementedException();
        }

        public Task SendRequest(string Request)
        {
            throw new NotImplementedException();
        }
    }
}
