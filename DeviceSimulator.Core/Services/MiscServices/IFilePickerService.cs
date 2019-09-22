using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DeviceSimulator.Core
{
    public interface IFilePickerService
    {
        Task<DeviceSetting> LoadDeviceSettingFromDiskAsync();

        Task SaveDeviceSettingFromDiskAsync(DeviceSetting deviceSetting);
    }
}
