using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DeviceSimulator.Core
{
    public interface ITranslationsService
    {
        Task LoadTranslations(string language = "en-US");

        string GetString(string key);
    }
}
