using MvvmCross.Plugin.Messenger;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessagePublisher.Core
{
    public class CloudToDeviceCommunicationViewModel : MvxViewModel
    {
        #region Fields

        private MvxSubscriptionToken _deviceStatusChangedMessageToken;

        private string _messageOutput = string.Empty;

        #endregion

        #region Constructors & Lifecycle

        public CloudToDeviceCommunicationViewModel(IMvxMessenger messageService)
        {
            messageService.Subscribe<CloudMessageReceivedMessage>(HandleCloudToDeviceMessage);
        }

        #endregion

        #region Public

        public string MessageOutput
        {
            get => _messageOutput;
            set
            {
                _messageOutput = value;
                RaisePropertyChanged(() => MessageOutput);
            }
        }

        #endregion

        #region Private

        private void HandleCloudToDeviceMessage (CloudMessageReceivedMessage message)
        {
            if (!string.IsNullOrEmpty(message.Message))
            {
                MessageOutput += $"{message.Message}\n";
            }
        }

        #endregion
    }
}
