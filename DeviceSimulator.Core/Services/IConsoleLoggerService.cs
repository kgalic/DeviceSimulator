using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceSimulator.Core
{
    public interface IConsoleLoggerService
    {
        void Log(string value);

        void LogDirectMethod(string value);
    }
}
