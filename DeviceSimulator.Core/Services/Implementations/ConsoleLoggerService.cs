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

        public void Log(string value)
        {
            _messageService.Publish(new DeviceStatusUpdatedMessage(this, value));
        }

        #endregion
    }
}
