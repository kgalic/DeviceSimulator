using MvvmCross.Commands;
using MvvmCross.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessagePublisher.Core
{
    public class DirectMethodSetting
    {
        public DirectMethodSetting(string directMethodName)
        {
            DirectMethodName = directMethodName;
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
    }
}
