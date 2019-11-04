using Microsoft.Azure.EventHubs;
using MvvmCross.Plugin.Messenger;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MessagePublisher.Core
{
    public class EventHubService : IEventHubService
    {
        #region Fields

        private readonly IMvxMessenger _messageService;
        private readonly IMessageExpressionService _messageExpressionService;
        private readonly IConsoleLoggerService _consoleLoggerService;
        private readonly ITranslationsService _translationsService;

        private EventHubClient _eventHubClient;
        private bool _isConnected;

        CancellationTokenSource _source;
        CancellationToken _cancellationToken;

        #endregion

        #region Constructors & Lifecycle

        public EventHubService(IMvxMessenger messageService,
                             IMessageExpressionService messageExpressionService,
                             IConsoleLoggerService consoleLoggerService,
                             ITranslationsService translationsService)
        {
            _messageExpressionService = messageExpressionService;
            _messageService = messageService;
            _consoleLoggerService = consoleLoggerService;
            _translationsService = translationsService;
        }

        #endregion

        #region IEventHubService

        public bool IsConnected => _isConnected;

        public Task Connect(string connectionString)
        {
            _source = new CancellationTokenSource();
            _cancellationToken = _source.Token;
            _eventHubClient = EventHubClient.CreateFromConnectionString(connectionString);
            _isConnected = true;

            _consoleLoggerService.Log(value: _translationsService.GetString("EventHubConnected"),
                                      logType: ConsoleLogTypes.EventHub);
            _isConnected = true;
            SendConnectionUpdatedMessage();
            return Task.FromResult(true);
        }

        public async Task Disconnect()
        {
            _source.Cancel();
            await _eventHubClient.CloseAsync();
            _isConnected = false;
            SendConnectionUpdatedMessage();
        }

        public async Task SendRequest(string message)
        {
            message = _messageExpressionService.ParseMessageExpressions(message);

            byte[] data = Encoding.UTF8.GetBytes(message);
            var messageRequest = new EventData(data);

            await _eventHubClient.SendAsync(messageRequest);

            var logMessage = string.Format(_translationsService.GetString("EventHubMessageSent"),
                                           Environment.NewLine,
                                           message,
                                           Environment.NewLine);

            _consoleLoggerService.Log(value: logMessage,
                                      logType: ConsoleLogTypes.EventHub);
        }

        #endregion

        #region Private Methods

        private void SendConnectionUpdatedMessage()
        {
            _messageService.Publish(new EventHubConnectionChangedMessage(this, IsConnected));
        }

        #endregion
    }
}
