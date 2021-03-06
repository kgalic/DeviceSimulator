﻿using MessagePublisher.Core.ViewModels;
using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.Plugin.Messenger;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessagePublisher.Core
{
    public class MainViewModel : MvxViewModel
    {
        #region Fields

        private readonly List<(string Tag, Type viewModel)> _pages = new List<(string Tag, Type Page)>
        {
            ("home", typeof(HomeViewModel)),
            ("direct_method", typeof(DirectMethodCommunicationViewModel)),
            ("c2d_messages", typeof(CloudToDeviceCommunicationViewModel)),
            ("event_grid", typeof(EventGridViewModel)),
            ("service_bus", typeof(ServiceBusViewModel)),
            ("event_hub", typeof(EventHubViewModel)),
            ("mqtt_publisher", typeof(MqttPublisherViewModel))
        };

        private readonly IDeviceService _deviceService;

        private MvxSubscriptionToken _deviceConnectionStatusChangedMessageToken;

        #endregion

        #region Constructors & Lifecycle

        public MainViewModel(IDeviceService deviceService,
                             IMvxMessenger messageService)
        {
            _deviceService = deviceService;
            _deviceConnectionStatusChangedMessageToken = messageService.Subscribe<DeviceConnectionChangedMessage>(HandleDeviceConnectionStatus);
        }

        public override void ViewAppeared()
        {
            base.ViewAppeared();
            Mvx.IoCProvider.Resolve<IMvxNavigationService>().Navigate<HomeViewModel>();
        }

        #endregion

        #region Public

        public bool IsIoTHubConnected
        {
            get => _deviceService.IsConnected;
        }

        #endregion

        #region Commands

        public IMvxCommand ItemInvokedCommand
        {
            get => new MvxCommand<string>((item) =>
            {
                var viewModel = _pages.FirstOrDefault(i => i.Tag == item);
                var type = viewModel.viewModel;
                if (type == typeof(HomeViewModel))
                {
                    Mvx.IoCProvider.Resolve<IMvxNavigationService>().Navigate<HomeViewModel> ();
                }
                else if (type == typeof(DirectMethodCommunicationViewModel))
                {
                    Mvx.IoCProvider.Resolve<IMvxNavigationService>().Navigate<DirectMethodCommunicationViewModel>();
                }
                else if (type == typeof(CloudToDeviceCommunicationViewModel))
                {
                    Mvx.IoCProvider.Resolve<IMvxNavigationService>().Navigate<CloudToDeviceCommunicationViewModel>();
                }
                else if (type == typeof(EventGridViewModel))
                {
                    Mvx.IoCProvider.Resolve<IMvxNavigationService>().Navigate<EventGridViewModel>();
                }
                else if (type == typeof(ServiceBusViewModel))
                {
                    Mvx.IoCProvider.Resolve<IMvxNavigationService>().Navigate<ServiceBusViewModel>();
                }
                else if (type == typeof(EventHubViewModel))
                {
                    Mvx.IoCProvider.Resolve<IMvxNavigationService>().Navigate<EventHubViewModel>();
                }
                else if (type == typeof(MqttPublisherViewModel))
                {
                    Mvx.IoCProvider.Resolve<IMvxNavigationService>().Navigate<MqttPublisherViewModel>();
                }
            });
        }

        #endregion

        #region Private

        private void HandleDeviceConnectionStatus(DeviceConnectionChangedMessage message)
        {
            RaisePropertyChanged(() => IsIoTHubConnected);
        }

        #endregion
    }
}
