using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MessagePublisher.Core
{
    public class TranslationsService : ITranslationsService
    {
        private readonly IDictionary<string, string> _paths =
            new Dictionary<string, string>()
            {
                {"en-US", @"Translations\Strings.xml" }
            };

        private readonly IDictionary<string, string> _translations;

        public TranslationsService()
        {
            _translations = new Dictionary<string, string>();
        }

        #region ITranslationService

        public string GetString(string key)
        {
            var value = string.Empty;
            _translations.TryGetValue(key, out value);
            if (string.IsNullOrEmpty(value))
            {
                return key;
            }
            else
            {
                return value;
            }
        }

        public Task LoadTranslations(string language = "en-US")
        {
            var path = string.Empty;
            _paths.TryGetValue(language, out path);
            if (!string.IsNullOrEmpty(path))
            {
                var document = XDocument.Load(path);
                var root = document.Root;
                foreach(var node in root.Elements())
                {
                    _translations.Add(node.FirstAttribute.Value, node.Value);
                }
            }

            return Task.FromResult(true);
        }

        #endregion
    }
}
