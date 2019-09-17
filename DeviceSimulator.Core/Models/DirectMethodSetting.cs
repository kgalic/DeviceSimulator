using MvvmCross.Commands;
using MvvmCross.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceSimulator.Core
{
    public class DirectMethodSetting : MvxNotifyPropertyChanged
    {
        public DirectMethodSetting(string directMethodName)
        {
            DirectMethodName = directMethodName;
            IsEnabled = true;
        }

        public string DirectMethodName
        {
            get;
            set;
        }

        public int Delay
        {
            get;
            set;
        }

        private bool _isEnabled;
        [JsonIgnore]
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                RaisePropertyChanged();
            }
        }

        private string _commandString;
        [JsonIgnore]
        public string CommandString
        {
            get => _commandString;
            set
            {
                _commandString = value;
                RaisePropertyChanged();
            }
        }

        public IMvxCommand RegisterCommand
        {
            get;
            set;
        }
    }
}
