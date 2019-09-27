using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessagePublisher.Core
{
    public interface ITimerService<T> where T : BasePublisherViewModel
    {
        bool IsRunning { get; }

        void Initialize();
    }
}
