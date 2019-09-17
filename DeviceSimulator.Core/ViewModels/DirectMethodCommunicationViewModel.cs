using DeviceSimulator.Core.Messages;
using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.Plugin.Messenger;
using MvvmCross.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace DeviceSimulator.Core
{
    public class DirectMethodCommunicationViewModel : MvxViewModel
    {
        #region Fields

        private readonly IDeviceService _deviceService;
        private readonly ITranslationsService _translationsService;

        private readonly ObservableCollection<DirectMethodSetting> _directMethods;

        private MvxSubscriptionToken _directMethodStatusChangedMessageToken;

        private string _directMethodEntry;
        private string _status = string.Empty;

        #endregion

        #region Constructors & Lifecycle

        public DirectMethodCommunicationViewModel(IDeviceService deviceService,
                                                  ITranslationsService translationsService,
                                                  IMvxMessenger messageService)
        {
            _directMethods = new ObservableCollection<DirectMethodSetting>();
            _deviceService = deviceService;
            _translationsService = translationsService;

            _directMethodStatusChangedMessageToken = messageService.Subscribe<DirectMethodStatusUpdatedMessage>(HandleDirectMethodStatusChanged);
        }

        #endregion

        #region Public

        public ObservableCollection<DirectMethodSetting> DirectMethods => _directMethods;

        public string DirectMethodEntry
        {
            get => _directMethodEntry;
            set
            {
                _directMethodEntry = value;
                RaisePropertyChanged();
            }
        }

        public string Status
        {
            get => _status;
            set
            {
                _status = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Translations

        public string DirectMethodNamePlaceholder
        {
            get => _translationsService.GetString("DirectMethodName");
        }

        public string AddDirectMethodString
        {
            get => _translationsService.GetString("AddDirectMethod");
        }

        public string DelayString
        {
            get => _translationsService.GetString("Delay");
        }

        #endregion

        #region Commands

        public IMvxCommand AddDirectMethodCommand
        {
            get => new MvxCommand(() =>
            {
                if (!string.IsNullOrEmpty(DirectMethodEntry))
                {
                    var directMethod = new DirectMethodSetting(DirectMethodEntry)
                    {
                        CommandString = _translationsService.GetString("RegisterMethod")
                    };
                    var command = new MvxCommand(async () =>
                    {
                        if (directMethod.IsEnabled)
                        {
                            directMethod.CommandString = _translationsService.GetString("UnregisterMethod"); 
                            directMethod.IsEnabled = false;
                            await _deviceService.RegisterDirectMethodAsync(directMethod);
                        }
                        else
                        {
                            directMethod.CommandString = _translationsService.GetString("RegisterMethod");
                            directMethod.IsEnabled = true;
                            await _deviceService.UnregisterDirectMethodAsync(directMethod.DirectMethodName);
                        }
                    });
                    directMethod.RegisterCommand = command;
                    _directMethods.Add(directMethod);
                    DirectMethodEntry = string.Empty;
                    RaisePropertyChanged(() => DirectMethods);
                }
            });
        }

        #endregion

        #region Private

        private void HandleDirectMethodStatusChanged(DirectMethodStatusUpdatedMessage message)
        {
            if (!string.IsNullOrEmpty(message.Status))
            {
                Status += message.Status;
            }
        }

        #endregion
    }
}
