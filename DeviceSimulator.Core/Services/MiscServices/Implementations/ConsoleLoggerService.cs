using DeviceSimulator.Core.Messages;
using MvvmCross.Plugin.Messenger;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceSimulator.Core
{
    public class ConsoleLoggerService : IConsoleLoggerService
    {
        #region Fields

        private readonly IMvxMessenger _messageService;

        #endregion

        #region Constructors & Lifecycle

        public ConsoleLoggerService(IMvxMessenger messageService)
        {
            _messageService = messageService;
        }

        #endregion

        #region IConsoleLoggerService

        public void Log(string value, Types logType)
        {
            switch (logType)
            {
                case Types.D2CCommunication:
                    _messageService.Publish(new DeviceStatusUpdatedMessage(this, value));
                    break;
                case Types.DirectMethodCommunication:
                    _messageService.Publish(new DirectMethodStatusUpdatedMessage(this, value));
                    break;
                case Types.C2DCommunication:
                    _messageService.Publish(new CloudMessageReceivedMessage(this, value));
                    break;
                case Types.EventGrid:
                    _messageService.Publish(new EventGridStatusUpdatedMessage(this, value));
                    break;
                default:
                    Console.WriteLine(value);
                    break;
            }
        }

        #endregion
    }
}
