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

        private DeviceClient _deviceClient;
        private bool _isConnected;

        private IDictionary<string, MethodCallback> _directMethodDictionary;

        public DeviceService(IMvxMessenger messageService,
                             IMessageExpressionService messageExpressionService,
                             IConsoleLoggerService consoleLoggerService)
        {
            _messageExpressionService = messageExpressionService;
            _messageService = messageService;
            _consoleLoggerService = consoleLoggerService;
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
                _consoleLoggerService.Log(string.Format("IoT hub cannot be reached\n{0}", e.Message));
                _isConnected = false;
            }

            SendDeviceConnectionChangedMessage();
        }

        public async Task DisconnectFromDevice()
        {
            await _deviceClient.CloseAsync();
            _isConnected = false;
            _consoleLoggerService.Log("Device Disconnected");

            SendDeviceConnectionChangedMessage();
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

            _consoleLoggerService.Log($"Message has been sent to IoT Hub\n{message}\n");
        }

        public async Task RegisterDirectMethodAsync(string methodName)
        {
            if (_directMethodDictionary == null)
            {
                _directMethodDictionary = new Dictionary<string, MethodCallback>();
            }
            if (!string.IsNullOrEmpty(methodName) && !_directMethodDictionary.ContainsKey(methodName))
            {
                MethodCallback callbackMethod = delegate (MethodRequest methodRequest, object userContext)
                {
                    _consoleLoggerService.Log($"Executed {methodName}\n");
                    return Task.FromResult(new MethodResponse(200));
                };
                _directMethodDictionary.Add(methodName, callbackMethod);

                await _deviceClient.SetMethodHandlerAsync(methodName: methodName, callbackMethod, null);
            }
        }

        private async Task InitializeDevice()
        {
            await _deviceClient.OpenAsync();
            _consoleLoggerService.Log("Device Connected");
            _isConnected = true;
        }

        private void SendDeviceConnectionChangedMessage()
        {
            _messageService.Publish(new DeviceConnectionChangedMessage(this, _isConnected));
        }
    }
}
