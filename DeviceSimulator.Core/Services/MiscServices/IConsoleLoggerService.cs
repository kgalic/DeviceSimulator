using System;
using System.Collections.Generic;
using System.Text;

namespace MessagePublisher.Core
{
    public interface IConsoleLoggerService
    {
        void Log(string value, ConsoleLogTypes logType);
    }

    public enum ConsoleLogTypes
    {
        D2CCommunication,
        DirectMethodCommunication,
        C2DCommunication,
        EventGrid,
        ServiceBus,
        EventHub,
        MqttPublisher
    }
}
