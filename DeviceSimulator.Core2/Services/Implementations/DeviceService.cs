using Microsoft.Azure.Devices.Client;
using MvvmCross.Plugin.Messenger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceSimulator.Core
{
    public class DeviceService : IDeviceService
    {
        private readonly IMvxMessenger _messageService;
        private DeviceClient _deviceClient;
        private string _deviceStatus;
        private bool _isConnected;

        public DeviceService(IMvxMessenger messageService)
        {
            this._messageService = messageService;
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
                _deviceStatus = string.Format("IoT hub cannot be reached\n{0}", e.Message);
                _isConnected = false;
            }
            SendDeviceUpadtedMessage();
            SendDeviceConnectionChangedMessage();
        }

        public async Task DisconnectFromDevice()
        {
            await _deviceClient.CloseAsync();
            _isConnected = false;
            _deviceStatus = "Device Disconnected";
            
            SendDeviceUpadtedMessage();
            SendDeviceConnectionChangedMessage();
        }

        public string Status => _deviceStatus;
       
        public bool IsConnected => _isConnected;

        public async Task SendRequest(string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            var messageRequest = new Message(data)
            {
                MessageId = Guid.NewGuid().ToString()
            };
            await _deviceClient.SendEventAsync(messageRequest);

            _deviceStatus = "Message has been sent to IoT Hub";
            SendDeviceUpadtedMessage();
        }

        private async Task InitializeDevice()
        {
            await _deviceClient.OpenAsync();
            _deviceStatus = "Device Connected";
            _isConnected = true;
        }

        private void SendDeviceUpadtedMessage()
        {
            _messageService.Publish(new DeviceStatusUpdatedMessage(this, _deviceStatus));
        }

        private void SendDeviceConnectionChangedMessage()
        {
            _messageService.Publish(new DeviceConnectionChangedMessage(this, _isConnected));
        }
    }
}
