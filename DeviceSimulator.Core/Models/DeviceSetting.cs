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
        public string Message
        {
            get;
            set;
        }

        public IList<DirectMethodSetting> DirectMethodSettings
        {
            get;
            set;
        }
    }
}
