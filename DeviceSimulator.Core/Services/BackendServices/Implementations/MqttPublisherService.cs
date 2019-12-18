using MvvmCross.Plugin.Messenger;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace MessagePublisher.Core
{
    public class MqttPublisherService : IMqttPublisherService
    {
        #region Fields

        private readonly IMvxMessenger _messageService;
        private readonly IMessageExpressionService _messageExpressionService;
        private readonly IConsoleLoggerService _consoleLoggerService;
        private readonly ITranslationsService _translationsService;

        private MqttClient _mqttClient;
        private bool _isConnected;
        private string _topic;

        CancellationTokenSource _source;
        CancellationToken _cancellationToken;

        #endregion

        #region Constructors & Lifecycle

        public MqttPublisherService(IMvxMessenger messageService,
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

        #region IMqttPublisherService implementation

        public bool IsConnected => _isConnected;

        public Task Connect(string hostName, string topic)
        {
            _source = new CancellationTokenSource();
            _cancellationToken = _source.Token;
            _topic = topic;
            _mqttClient = new MqttClient(hostName);

            var clientId = Guid.NewGuid().ToString();
            _mqttClient.Connect(clientId);

            _consoleLoggerService.Log(value: _translationsService.GetString("MqttPublisherConnected"),
                                      logType: ConsoleLogTypes.MqttPublisher);
            _isConnected = true;
            SendConnectionUpdatedMessage();
            return Task.FromResult(true);
        }

        public Task Disconnect()
        {
            _mqttClient.Disconnect();
            _mqttClient = null;
            _isConnected = false;
            SendConnectionUpdatedMessage();
            return Task.FromResult(true);
        }

        public Task SendRequest(string request)
        {
            if (_isConnected)
            {
                request = _messageExpressionService.ParseMessageExpressions(request);

                _mqttClient.Publish(topic: _topic,
                                    message: Encoding.UTF8.GetBytes(request),
                                    qosLevel: MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE,
                                    retain: false);

                var logMessage = string.Format(_translationsService.GetString("MqttPublisherMessageSent"),
                                              Environment.NewLine,
                                              request,
                                              Environment.NewLine);

                _consoleLoggerService.Log(value: logMessage,
                                          logType: ConsoleLogTypes.MqttPublisher);
            }

            return Task.FromResult(true);
        }

        #endregion

        #region Private

        private void SendConnectionUpdatedMessage()
        {
            _messageService.Publish(new MqttPublisherConnectionChangedMessage(this, IsConnected));
        }

        #endregion
    }
}
