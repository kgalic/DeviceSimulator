using System;
using System.Threading.Tasks;
using MessagePublisher.Core;

namespace MessagePublisher.MAC
{
    public class FilePickerService : IFilePickerService
    {
        public FilePickerService()
        {
        }

        public Task<string> LoadSettingsFromDiskAsync()
        {
            throw new NotImplementedException();
        }

        public Task SaveDeviceSettingFromDiskAsync(BaseSetting deviceSetting)
        {
            throw new NotImplementedException();
        }
    }
}
