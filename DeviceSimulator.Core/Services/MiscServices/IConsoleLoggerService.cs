using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceSimulator.Core
{
    public interface IConsoleLoggerService
    {
        void Log(string value, Types logType);
    }

    public enum Types
    {
        D2CCommunication,
        DirectMethodCommunication,
        C2DCommunication,
        EventGrid
    }
}
