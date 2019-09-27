using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MessagePublisher.Core
{
    public class ServiceBusPublisherService : IServiceBusPublisherService
    {
        #region Fields

        private readonly IMessageExpressionService _messageExpressionService;
        private readonly ITranslationsService _translationsService;
        private readonly IConsoleLoggerService _consoleLoggerService;
        private ServiceBusPublisherTypes _serviceBusPublisherTypes;

        private ISenderClient _senderClient;

        #endregion

        #region Constructors

        public ServiceBusPublisherService(IMessageExpressionService messageExpressionService,
                                          IConsoleLoggerService consoleLoggerService,
                                          ITranslationsService translationsService)
        {
            _messageExpressionService = messageExpressionService;
            _translationsService = translationsService;
            _consoleLoggerService = consoleLoggerService;
        }

        #endregion

        #region IServiceBusPublisherService

        public bool IsConnected { get; private set; }

        public Task Connect(string connectionString, 
                            string entity, 
                            ServiceBusPublisherTypes serviceBusPublisherType)
        {
            _serviceBusPublisherTypes = serviceBusPublisherType;
            if (serviceBusPublisherType == ServiceBusPublisherTypes.Queue)
            {
                _senderClient = new QueueClient(connectionString, entity);
            }
            else
            {
                _senderClient = new TopicClient(connectionString, entity);
            }
            IsConnected = true;

            _consoleLoggerService.Log(value: _translationsService.GetString("ServiceBusConnected"),
                                      logType: ConsoleLogTypes.ServiceBus);

            return Task.FromResult(true);
        }

        public async Task Disconnect()
        {
            await _senderClient.CloseAsync();
            IsConnected = false;
            _senderClient = null;

            _consoleLoggerService.Log(value: _translationsService.GetString("ServiceBusDisconnected"),
                                      logType: ConsoleLogTypes.ServiceBus);
        }

        public async Task SendRequest(string request)
        {
            request = _messageExpressionService.ParseMessageExpressions(request);

            var message = new Message(UTF8Encoding.UTF8.GetBytes(request));
            await _senderClient.SendAsync(message);
            var logMessage = string.Format(_translationsService.GetString("ServiceBusMessageSent"),
                                           Environment.NewLine,
                                           request,
                                           Environment.NewLine);

            _consoleLoggerService.Log(value: logMessage,
                                      logType: ConsoleLogTypes.ServiceBus);

        }

        #endregion
    }
}
