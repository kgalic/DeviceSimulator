using MvvmCross.Commands;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessagePublisher.Core
{
    public class DirectMethodSettingViewItem : MvxNotifyPropertyChanged
    {
        private DirectMethodSetting _directMethodSetting;
        public DirectMethodSetting DirectMethodSetting
        {
            get => _directMethodSetting;
            set
            {
                _directMethodSetting = value;
                RaisePropertyChanged();
            }
        }

        private bool _isEnabled = true;
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

        public IMvxCommand RemoveCommand
        {
            get;
            set;
        }
    }
}
