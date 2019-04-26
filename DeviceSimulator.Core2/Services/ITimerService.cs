using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceSimulator.Core
{
    public interface ITimerService
    {
        bool IsRunning { get; }

        void Initialize();
    }
}
