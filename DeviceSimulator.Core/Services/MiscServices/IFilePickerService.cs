using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MessagePublisher.Core
{
    public interface IFilePickerService
    {
        Task<string> LoadSettingsFromDiskAsync();

        Task SaveDeviceSettingFromDiskAsync(BaseSetting deviceSetting);
    }
}
