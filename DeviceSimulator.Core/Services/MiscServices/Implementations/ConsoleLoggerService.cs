using MessagePublisher.Core.Messages;
using MvvmCross.Plugin.Messenger;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessagePublisher.Core
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

        public void Log(string value, ConsoleLogTypes logType)
        {
            switch (logType)
            {
                case ConsoleLogTypes.D2CCommunication:
                    _messageService.Publish(new DeviceStatusUpdatedMessage(this, value));
                    break;
                case ConsoleLogTypes.DirectMethodCommunication:
                    _messageService.Publish(new DirectMethodStatusUpdatedMessage(this, value));
                    break;
                case ConsoleLogTypes.C2DCommunication:
                    _messageService.Publish(new CloudMessageReceivedMessage(this, value));
                    break;
                case ConsoleLogTypes.EventGrid:
                    _messageService.Publish(new EventGridStatusUpdatedMessage(this, value));
                    break;
                case ConsoleLogTypes.ServiceBus:
                    _messageService.Publish(new ServiceBusStatusUpdatedMessage(this, value));
                    break;
                case ConsoleLogTypes.EventHub:
                    _messageService.Publish(new EventHubStatusUpdatedMessage(this, value));
                    break;
                case ConsoleLogTypes.MqttPublisher:
                    _messageService.Publish(new MqttPublisherStatusUpdatedMessage(this, value));
                    break;
                default:
                    Console.WriteLine(value);
                    break;
            }
        }

        #endregion
    }
}
