using DeviceSimulator.Core.Messages;
using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.Plugin.Messenger;
using MvvmCross.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceSimulator.Core
{
    public class DirectMethodCommunicationViewModel : MvxViewModel
    {
        #region Fields

        private readonly IDeviceService _deviceService;
        private readonly ITranslationsService _translationsService;
        private readonly IDeviceSettingDataService _deviceSettingDataService;

        private ObservableCollection<DirectMethodSettingViewItem> _directMethodSettingViewItems;

        private MvxSubscriptionToken _directMethodStatusChangedMessageToken;

        private string _directMethodEntry;
        private string _status = string.Empty;

        #endregion

        #region Constructors & Lifecycle

        public DirectMethodCommunicationViewModel(IDeviceService deviceService,
                                                  ITranslationsService translationsService,
                                                  IDeviceSettingDataService deviceSettingDataService,
                                                  IMvxMessenger messageService)
        {
            _deviceService = deviceService;
            _translationsService = translationsService;
            _deviceSettingDataService = deviceSettingDataService;

            _directMethodStatusChangedMessageToken = messageService.Subscribe<DirectMethodStatusUpdatedMessage>(HandleDirectMethodStatusChanged);
        }

        public override Task Initialize()
        {
            CreateViewItems();
            return base.Initialize();
        }

        public override void ViewDisappearing()
        {
            base.ViewDisappearing();
            _deviceSettingDataService.DeviceSetting.DirectMethodSettings = _directMethodSettingViewItems.Select(item => item.DirectMethodSetting).ToList();
        }

        #endregion

        #region Public

        public ObservableCollection<DirectMethodSettingViewItem> DirectMethods => _directMethodSettingViewItems;

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
                    var directMethod = new DirectMethodSettingViewItem()
                    {
                        DirectMethodSetting = new DirectMethodSetting(DirectMethodEntry),
                        CommandString = _translationsService.GetString("RegisterMethod")
                    };
                    var command = new MvxCommand(async () =>
                    {
                        if (directMethod.IsEnabled)
                        {
                            directMethod.CommandString = _translationsService.GetString("UnregisterMethod"); 
                            directMethod.IsEnabled = false;
                            await _deviceService.RegisterDirectMethodAsync(directMethod.DirectMethodSetting);
                        }
                        else
                        {
                            directMethod.CommandString = _translationsService.GetString("RegisterMethod");
                            directMethod.IsEnabled = true;
                            await _deviceService.UnregisterDirectMethodAsync(directMethod.DirectMethodSetting.DirectMethodName);
                        }
                    });
                    directMethod.RegisterCommand = command;
                    _directMethodSettingViewItems.Add(directMethod);
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

        private void CreateViewItems()
        {
            _directMethodSettingViewItems = new ObservableCollection<DirectMethodSettingViewItem>();
            var directMethodSettings = _deviceSettingDataService.DeviceSetting.DirectMethodSettings;
            if (directMethodSettings != null)
            {
                foreach(var item in directMethodSettings)
                {
                    var viewItem = new DirectMethodSettingViewItem()
                    {
                        DirectMethodSetting = item
                    };
                    _directMethodSettingViewItems.Add(viewItem);
                }
            }
        }

        #endregion
    }
}
