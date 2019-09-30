using MessagePublisher.Core.Messages;
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

namespace MessagePublisher.Core
{
    public class DirectMethodCommunicationViewModel : MvxViewModel
    {
        #region Fields

        private readonly IDeviceService _deviceService;
        private readonly ITranslationsService _translationsService;
        private readonly IDeviceSettingDataService _deviceSettingDataService;

        private ObservableCollection<DirectMethodSettingViewItem> _directMethodSettingViewItems;

        private MvxSubscriptionToken _deviceConnectionStatusChangedMessageToken;
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
            _deviceConnectionStatusChangedMessageToken = messageService.Subscribe<DeviceConnectionChangedMessage>(HandleDeviceConnectionStatus);
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

        public ObservableCollection<DirectMethodSettingViewItem> ViewItems
        {
            get => _directMethodSettingViewItems;
            set
            {
                _directMethodSettingViewItems = value;
                RaisePropertyChanged();
            }
        }

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
                    if (_directMethodSettingViewItems.FirstOrDefault(item => item.DirectMethodSetting.DirectMethodName == DirectMethodEntry) == null)
                    {
                        var directMethodSetting = new DirectMethodSetting(DirectMethodEntry);
                        var directMethodViewItem = CreateDirectMethodViewItem(directMethodSetting);
                        _directMethodSettingViewItems.Add(directMethodViewItem);
                        DirectMethodEntry = string.Empty;
                        RaisePropertyChanged(() => ViewItems);
                    }
                    else
                    {
                        Mvx.IoCProvider.Resolve<IConsoleLoggerService>().Log(value: _translationsService.GetString("ExistingMethod"),
                                                                             logType: ConsoleLogTypes.DirectMethodCommunication);
                    }
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
                    var viewItem = CreateDirectMethodViewItem(item);
                    _directMethodSettingViewItems.Add(viewItem);
                }
            }
        }

        private DirectMethodSettingViewItem CreateDirectMethodViewItem(DirectMethodSetting directMethodSetting)
        {
            var directMethodViewItem = new DirectMethodSettingViewItem()
            {
                DirectMethodSetting = directMethodSetting,
                CommandString = _translationsService.GetString("RegisterMethod")
            };
            var registerCommand = new MvxCommand(async () =>
            {
                if (directMethodViewItem.IsEnabled)
                {
                    directMethodViewItem.CommandString = _translationsService.GetString("UnregisterMethod");
                    directMethodViewItem.IsEnabled = false;
                    await _deviceService.RegisterDirectMethodAsync(directMethodViewItem.DirectMethodSetting);
                }
                else
                {
                    directMethodViewItem.CommandString = _translationsService.GetString("RegisterMethod");
                    directMethodViewItem.IsEnabled = true;
                    await _deviceService.UnregisterDirectMethodAsync(directMethodViewItem.DirectMethodSetting.DirectMethodName);
                }
            });
            var removeCommand = new MvxCommand<DirectMethodSettingViewItem>(async (item) =>
            {
                await _deviceService.UnregisterDirectMethodAsync(item.DirectMethodSetting.DirectMethodName);
                _directMethodSettingViewItems.Remove(item);
            });
            directMethodViewItem.RemoveCommand = removeCommand;
            directMethodViewItem.RegisterCommand = registerCommand;

            return directMethodViewItem;
        }

        private void HandleDeviceConnectionStatus(DeviceConnectionChangedMessage message)
        {
            if (message.IsConnected)
            {
                Status = string.Empty;
            }
            else
            {
                foreach(var viewItem in ViewItems)
                {
                    if (!viewItem.IsEnabled)
                    {
                        viewItem.RegisterCommand.Execute();
                    }
                }
            }
        }

        #endregion
    }
}
