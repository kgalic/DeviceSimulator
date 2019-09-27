using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessagePublisher.Core
{   public interface IDeviceService : IPublisherService
    {
        Task Connect(string connectionString);

        Task RegisterDirectMethodAsync(DirectMethodSetting directMethod);

        Task UnregisterDirectMethodAsync(string methodName);
    }
}
