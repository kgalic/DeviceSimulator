using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceSimulator.Core
{
    public class DeviceSetting
    {
        public string ConnectionString
        {
            get;
            set;
        }

        public DirectMethodSetting DirectMethodSetting
        {
            get;
            set;
        }
    }
}
