using Microsoft.Azure.Devices.Client;
using MvvmCross.Plugin.Messenger;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceSimulator.Core
{
    public class DeviceService : IDeviceService
    {
        private readonly IMvxMessenger _messageService;
        private readonly IMessageExpressionService _messageExpressionService;
        private readonly IConsoleLoggerService _consoleLoggerService;
        private readonly ITranslationsService _translationsService;

        private DeviceClient _deviceClient;
        private bool _isConnected;

        private IDictionary<string, MethodCallback> _directMethodDictionary;

        public DeviceService(IMvxMessenger messageService,
                             IMessageExpressionService messageExpressionService,
                             IConsoleLoggerService consoleLoggerService,
                             ITranslationsService translationsService)
        {
            _messageExpressionService = messageExpressionService;
            _messageService = messageService;
            _consoleLoggerService = consoleLoggerService;
            _translationsService = translationsService;
        }

        public async Task ConnectToDevice(string connectionString)
        {
            try
            {
                _deviceClient = DeviceClient.CreateFromConnectionString(connectionString);

                await InitializeDevice();
            }
            catch (Exception e)
            {
                _consoleLoggerService.Log(string.Format(_translationsService.GetString("IoTHubNotReachableMessageException"), 
                                                        Environment.NewLine,
                                                        e.Message));
                _isConnected = false;
            }
            SendDeviceConnectionUpdatedMessage();
        }

        public async Task DisconnectFromDevice()
        {
            await _deviceClient.CloseAsync();
            _isConnected = false;

            SendDeviceConnectionUpdatedMessage();

            _consoleLoggerService.Log(_translationsService.GetString("DeviceDisconnected"));
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

            _consoleLoggerService.Log(string.Format(_translationsService.GetString("MessageSent"), 
                                                    Environment.NewLine,
                                                    message,
                                                    Environment.NewLine));
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
                    _consoleLoggerService.LogDirectMethod(string.Format(_translationsService.GetString("MethodExecuting"),
                                                          methodName,
                                                          Environment.NewLine));

                    await Task.Delay(TimeSpan.FromSeconds(directMethod.Delay));

                    _consoleLoggerService.LogDirectMethod(string.Format(_translationsService.GetString("MethodExecuted"), 
                                                          methodName,
                                                          Environment.NewLine));

                    return new MethodResponse(200);
                };

                await _deviceClient.SetMethodHandlerAsync(methodName: methodName, callbackMethod, null);

                _directMethodDictionary.Add(methodName, callbackMethod);
                _consoleLoggerService.LogDirectMethod(string.Format(_translationsService.GetString("MethodRegistered"),
                                                      methodName,
                                                      Environment.NewLine));
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
                _consoleLoggerService.LogDirectMethod(string.Format(_translationsService.GetString("MethodUnregistered"),
                                                      methodName,
                                                      Environment.NewLine));
            }
        }

        private async Task InitializeDevice()
        {
            await _deviceClient.OpenAsync();
            _consoleLoggerService.Log(_translationsService.GetString("DeviceConnected"));
            _isConnected = true;
        }

        private void SendDeviceConnectionUpdatedMessage()
        {
            _messageService.Publish(new DeviceConnectionChangedMessage(this, IsConnected));
        }
    }
}
