using Newtonsoft.Json.Linq;
using System;
using System.Globalization;

namespace MessagePublisher.Core
{
    public class MessageExpressionService : IMessageExpressionService
    {
        private const string RandomKeyWord = "rnd";
        private const string DateTimeKeyWord = "datetime:current";
        private const char Splitter = ':';
        private const string Comma = ".";

        public string ParseMessageExpressions(string message)
        {
            JObject json = JObject.Parse(message);

            foreach (var property in json.Properties())
            {
                var value = property.Value.ToString();
                if (value.Contains(RandomKeyWord))
                {
                    var values = value.Split(new char[] { Splitter });
                    var minValue = values[1];
                    var maxValue = values[2];
                    if (minValue.Contains(Comma) || maxValue.Contains(Comma))
                    {
                        var minimum = double.Parse(minValue, CultureInfo.InvariantCulture);
                        var maximum = double.Parse(maxValue, CultureInfo.InvariantCulture);
                        Random random = new Random();
                        var randomValue = random.NextDouble() * (maximum - minimum) + minimum;
                        property.Value = randomValue;
                    }
                    else
                    {
                        var minimum = int.Parse(minValue);
                        var maximum = int.Parse(maxValue);
                        Random random = new Random();
                        var randomValue = random.Next(minimum, maximum);
                        property.Value = randomValue;
                    }
                }
                else if (value.Contains(DateTimeKeyWord))
                {
                    var dateValue = DateTime.Now.ToString("u").Replace(' ', 'T');
                    property.Value = dateValue;
                }
            }

            message = json.ToString();
            return message;
        }
    }
}
