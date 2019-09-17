using MvvmCross;
using MvvmCross.Commands;
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

        private readonly ObservableCollection<DirectMethodSetting> _directMethods;

        private string _directMethodEntry;

        #endregion

        #region Constructors & Lifecycle

        public DirectMethodCommunicationViewModel(IDeviceService deviceService)
        {
            _directMethods = new ObservableCollection<DirectMethodSetting>();
            _deviceService = deviceService;
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
                        CommandString = "Register"
                    };
                    var command = new MvxCommand(() =>
                    {
                        var aa = directMethod.Delay;
                        if (directMethod.IsEnabled)
                        {
                            directMethod.CommandString = "Unregister";
                            directMethod.IsEnabled = false;
                            _deviceService.RegisterDirectMethodAsync(DirectMethodEntry);
                        }
                        else
                        {
                            directMethod.CommandString = "Register";
                            directMethod.IsEnabled = true;
                            _deviceService.UnregisterDirectMethodAsync(DirectMethodEntry);
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
    }
}
