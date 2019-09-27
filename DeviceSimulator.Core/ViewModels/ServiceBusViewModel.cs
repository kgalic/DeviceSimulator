using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.Plugin.Messenger;
using Newtonsoft.Json;

namespace DeviceSimulator.Core
{
    public class ServiceBusViewModel : BasePublisherViewModel
    {
        #region Fields

        protected readonly ITimerService<ServiceBusViewModel> _timerService;

        private MvxSubscriptionToken _statusChangedMessageToken;

        private ServiceBusSetting _serviceBusSetting;

        private IDictionary<string, ServiceBusPublisherTypes> _serviceBusEntityOptionsDictionary;

        #endregion

        #region Constructor & Lifecycle 

        public ServiceBusViewModel(IServiceBusPublisherService serviceBusPublisherService)
        {
            _publisherService = serviceBusPublisherService;
            _timerService = Mvx.IoCProvider.Resolve<ITimerService<ServiceBusViewModel>>();

            _serviceBusSetting = new ServiceBusSetting();

            SetConnectionStatus();
            InitializeServiceBusEntityOptions();

            ConsoleLogType = ConsoleLogTypes.ServiceBus;

            _statusChangedMessageToken = _messageService.Subscribe<ServiceBusStatusUpdatedMessage>(HandleStatusUpdatedMessage);
            _timerServiceTriggeredMessageToken = _messageService.Subscribe<TimerServiceTriggeredMessage<ServiceBusViewModel>>(HandleTimerTrigger);
        }

        #endregion

        #region Public 

        public string ConnectionString
        {
            get => _serviceBusSetting.ConnectionString;
            set
            {
                _serviceBusSetting.ConnectionString = value;
                RaisePropertyChanged();
            }
        }

        public string EntityName
        {
            get => _serviceBusSetting.EntityName;
            set
            {
                _serviceBusSetting.EntityName = value;
                RaisePropertyChanged();
            }
        }

        public int ServiceBusEntityOptionsSelectedIndexId
        {
            get => (int)_serviceBusSetting.Type;
            set
            {
                _serviceBusSetting.Type = (ServiceBusPublisherTypes)value;
                RaisePropertyChanged();
            }
        }

        private string _serviceBusEntityOptionsHeader;
        public string ServiceBusEntityOptionsHeader
        {
            get => _serviceBusEntityOptionsHeader;
            set
            {
                _serviceBusEntityOptionsHeader = value;
                RaisePropertyChanged();
            }
        }

        public override string MessagePayload
        {
            get => _serviceBusSetting.Message;
            set
            {
                _serviceBusSetting.Message = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<string> ServiceBusEntityOptions
        {
            get;
            private set;
        }

        public override bool IsRunning => _timerService.IsRunning;

        #endregion

        #region Translations

        public string ConnectionStringPlaceholder => _translationsService.GetString("ServiceBusConnectionStringPlaceholder");

        public string EntityNamePlaceholder => _translationsService.GetString("EntityNamePlaceholder");

        #endregion

        #region Commands

        public override IMvxCommand ConnectCommand
        {
            get
            {
                return new MvxCommand(async () =>
                {
                    try
                    {
                        if (ServiceBusPublisherServiceInstance.IsConnected)
                        {
                            await ServiceBusPublisherServiceInstance.Disconnect()
                                                                    .ConfigureAwait(false); ;
                        }
                        else
                        {
                            await ServiceBusPublisherServiceInstance.Connect(connectionString: ConnectionString,
                                                                             entity: EntityName,
                                                                             serviceBusPublisherType: _serviceBusSetting.Type)
                                                                    .ConfigureAwait(false);
                        }
                        SetConnectionStatus();
                    }
                    catch
                    {
                        _consoleLoggerService.Log(value: _translationsService.GetString("ParametersNotValid"), 
                                                  logType: ConsoleLogTypes.ServiceBus);
                    }
                });
            }
        }

        public override IMvxCommand SaveSettingsCommand
        {
            get
            {
                return new MvxCommand(async () =>
                {
                    await _filePickerService.SaveDeviceSettingFromDiskAsync(_serviceBusSetting);
                });
            }
        }

        public override IMvxCommand LoadSettingsCommand
        {
            get
            {
                return new MvxCommand(async () =>
                {
                    try
                    {
                        var serviceBusSettingString = await _filePickerService.LoadSettingsFromDiskAsync();
                        var serviceBusSetting = JsonConvert.DeserializeObject<ServiceBusSetting>(serviceBusSettingString);
                        if (serviceBusSetting != null)
                        {
                            await ResetAll();
                            MessagePayload = serviceBusSetting.Message;
                            EntityName = serviceBusSetting.EntityName;
                            ConnectionString = serviceBusSetting.ConnectionString;
                        }
                    }
                    catch
                    {
                        var exceptionMessage = _translationsService.GetString("ErrorLoadingFileMessageException");
                        _consoleLoggerService.Log(value: exceptionMessage,
                                                  logType: ConsoleLogType);
                    }
                });
            }
        }

        #endregion

        #region Private

        private IServiceBusPublisherService ServiceBusPublisherServiceInstance => _publisherService as IServiceBusPublisherService;

        #endregion

        #region ProtectedMethods

        protected override void StartTimer()
        {
            _isTimerOn = true;
            _messageService.Publish(new StartTimerServiceMessage<ServiceBusViewModel>(this, DelayInMiliseconds));
            TimerStatusTitle = _translationsService.GetString("StopTimer");
        }

        protected override void StopTimer()
        {
            _isTimerOn = false;
            _messageService.Publish(new StopTimerServiceMessage<ServiceBusViewModel>(this));
            TimerStatusTitle = _translationsService.GetString("StartTimer");
        }

        #endregion

        #region Private Methods
        private void HandleStatusUpdatedMessage(ServiceBusStatusUpdatedMessage message)
        {
            if (!string.IsNullOrEmpty(message.Status))
            {
                OutputLog += $"{message.Status}\n";
            }
        }

        protected void HandleTimerTrigger(TimerServiceTriggeredMessage<ServiceBusViewModel> message)
        {
            SendMessagePayload();
        }

        private void InitializeServiceBusEntityOptions()
        {
            var ServiceBusEntityOptions1 = new Dictionary<string, ServiceBusPublisherTypes>()
            {
                { _translationsService.GetString("Queue"), ServiceBusPublisherTypes.Queue },
                { _translationsService.GetString("Topic"), ServiceBusPublisherTypes.Topic },
            };

            ServiceBusEntityOptions = new ObservableCollection<string>()
            {
                _translationsService.GetString("Queue"),
                _translationsService.GetString("Topic")
            };

            ServiceBusEntityOptionsSelectedIndexId = 0;
        }

        #endregion
    }
}
