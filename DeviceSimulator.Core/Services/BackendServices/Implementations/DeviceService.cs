using Microsoft.Azure.Devices.Client;
using MvvmCross.Plugin.Messenger;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DeviceSimulator.Core
{
    public class DeviceService : IDeviceService
    {
        #region Fields

        private readonly IMvxMessenger _messageService;
        private readonly IMessageExpressionService _messageExpressionService;
        private readonly IConsoleLoggerService _consoleLoggerService;
        private readonly ITranslationsService _translationsService;

        private DeviceClient _deviceClient;
        private bool _isConnected;

        CancellationTokenSource _source = new CancellationTokenSource();
        CancellationToken _cancellationToken;

        private IDictionary<string, MethodCallback> _directMethodDictionary;

        #endregion

        #region Constructors

        public DeviceService(IMvxMessenger messageService,
                             IMessageExpressionService messageExpressionService,
                             IConsoleLoggerService consoleLoggerService,
                             ITranslationsService translationsService)
        {
            _messageExpressionService = messageExpressionService;
            _messageService = messageService;
            _consoleLoggerService = consoleLoggerService;
            _translationsService = translationsService;

            _cancellationToken = _source.Token;
        }

        #endregion

        #region IDeviceService

        public async Task Connect(string connectionString)
        {
            try
            {
                _deviceClient = DeviceClient.CreateFromConnectionString(connectionString);
                await InitializeDevice();
                ReceiveCloudMessages(_cancellationToken);
            }
            catch (Exception e)
            {
                var exceptionMessage = string.Format(_translationsService.GetString("IoTHubNotReachableMessageException"),
                                                        Environment.NewLine,
                                                        e.Message);

                _consoleLoggerService.Log(value: exceptionMessage,
                                          logType:Types.D2CCommunication);

                _isConnected = false;
            }
            SendDeviceConnectionUpdatedMessage();
        }

        public async Task Disconnect()
        {
            _source.Cancel();
            await _deviceClient.CloseAsync();
            _isConnected = false;

            SendDeviceConnectionUpdatedMessage();

            _consoleLoggerService.Log(_translationsService.GetString("DeviceDisconnected"),
                                      Types.D2CCommunication);
        }
       
        public bool IsConnected => _isConnected;

        public async Task SendRequest(string message)
        {
            message = _messageExpressionService.ParseMessageExpressions(message);

            byte[] data = Encoding.UTF8.GetBytes(message);
            var messageRequest = new Message(data)
            {
                MessageId = Guid.NewGuid().ToString()
            };

            await _deviceClient.SendEventAsync(messageRequest);

            var logMessage = string.Format(_translationsService.GetString("MessageSent"),
                                           Environment.NewLine,
                                           message,
                                           Environment.NewLine);

            _consoleLoggerService.Log(value: logMessage,
                                      logType: Types.D2CCommunication);
        }

        public async Task RegisterDirectMethodAsync(DirectMethodSetting directMethod)
        {
            if (_directMethodDictionary == null)
            {
                _directMethodDictionary = new Dictionary<string, MethodCallback>();
            }

            var methodName = directMethod.DirectMethodName;
            if (!string.IsNullOrEmpty(methodName) && !_directMethodDictionary.ContainsKey(methodName))
            {
                MethodCallback callbackMethod = async delegate (MethodRequest methodRequest, object userContext)
                {
                    var logOutputMessage = string.Format(_translationsService.GetString("MethodExecuting"),
                                                   methodName,
                                                   Environment.NewLine);
                    _consoleLoggerService.Log(value: logOutputMessage,
                                              logType: Types.DirectMethodCommunication);

                    await Task.Delay(TimeSpan.FromSeconds(directMethod.Delay));

                    logOutputMessage = string.Format(_translationsService.GetString("MethodExecuted"),
                                               methodName,
                                               Environment.NewLine);
                    _consoleLoggerService.Log(value: logOutputMessage,
                                              logType: Types.DirectMethodCommunication);

                    return new MethodResponse(200);
                };

                await _deviceClient.SetMethodHandlerAsync(methodName: methodName, callbackMethod, null);

                _directMethodDictionary.Add(methodName, callbackMethod);

                var logMessage = string.Format(_translationsService.GetString("MethodRegistered"),
                                               methodName,
                                               Environment.NewLine);
                _consoleLoggerService.Log(value: logMessage,
                                          logType: Types.DirectMethodCommunication);
            }
        }

        public async Task UnregisterDirectMethodAsync(string methodName)
        {
            if (_directMethodDictionary == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(methodName) && _directMethodDictionary.ContainsKey(methodName))
            {
                await _deviceClient.SetMethodHandlerAsync(methodName: methodName, null, null);
                _directMethodDictionary.Remove(methodName);

                var logMessage = string.Format(_translationsService.GetString("MethodUnregistered"),
                                               methodName,
                                               Environment.NewLine);
                _consoleLoggerService.Log(value: logMessage, 
                                          logType: Types.DirectMethodCommunication);
            }
        }

        #endregion

        #region Private Methods

        private async Task InitializeDevice()
        {
            await _deviceClient.OpenAsync();
            _consoleLoggerService.Log(value: _translationsService.GetString("DeviceConnected"),
                                      logType: Types.D2CCommunication);
            _isConnected = true;
        }

        private void SendDeviceConnectionUpdatedMessage()
        {
            _messageService.Publish(new DeviceConnectionChangedMessage(this, IsConnected));
        }

        private async Task ReceiveCloudMessages(CancellationToken cancellationToken)
        {
            while(!cancellationToken.IsCancellationRequested)
            {
                var cloudToDeviceMessage = await _deviceClient.ReceiveAsync(_cancellationToken);
                if (cloudToDeviceMessage.BodyStream != null)
                {
                    var streamReader = new StreamReader(cloudToDeviceMessage.BodyStream);
                    var message = await streamReader.ReadToEndAsync();

                    _consoleLoggerService.Log(value: message,
                                              logType: Types.C2DCommunication);
                }
            }
        }

        #endregion
    }
}
