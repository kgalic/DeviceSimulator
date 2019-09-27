using MessagePublisher.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage.Streams;

namespace MessagePublisher
{
    public class FilePickerService : IFilePickerService
    {
        private const string FileTypeFilterJsonExtension = ".json";

        private readonly ITranslationsService _translationsService;

        public FilePickerService(ITranslationsService translationsService)
        {
            _translationsService = translationsService;
        }

        public async Task<string> LoadSettingsFromDiskAsync()
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            picker.FileTypeFilter.Add(FileTypeFilterJsonExtension);

            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                var content = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);

                var reader = new DataReader(content.GetInputStreamAt(0));
                var bytes = new byte[content.Size];
                await reader.LoadAsync((uint)content.Size);
                reader.ReadBytes(bytes);

                string jsonStr = Encoding.UTF8.GetString(bytes);
                return jsonStr;
            }
            else
            {
                return null;
            }
        }

        public async Task SaveDeviceSettingFromDiskAsync(BaseSetting deviceSetting)
        {
            var savePicker = new Windows.Storage.Pickers.FileSavePicker();
            savePicker.SuggestedStartLocation =
                Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;

            // Dropdown of file types the user can save the file as
            savePicker.FileTypeChoices.Add(_translationsService.GetString("JsonConfig"), 
                                           new List<string>() { FileTypeFilterJsonExtension });

            // Default file name if the user does not type one in or select a file to replace
            savePicker.SuggestedFileName = _translationsService.GetString("DeviceConfigFileName");

            Windows.Storage.StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                // Prevent updates to the remote version of the file until
                // we finish making changes and call CompleteUpdatesAsync.
                Windows.Storage.CachedFileManager.DeferUpdates(file);

                var deviceSettingJsonString = JsonConvert.SerializeObject(deviceSetting);

                // write to file
                await Windows.Storage.FileIO.WriteTextAsync(file, deviceSettingJsonString);

                // Let Windows know that we're finished changing the file so
                // the other app can update the remote version of the file.
                // Completing updates may require Windows to ask for user input.
                await Windows.Storage.CachedFileManager.CompleteUpdatesAsync(file);
            }
        }
    }
}
