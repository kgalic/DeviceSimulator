using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DeviceSimulator.Core
{
    public class EventGridService : IEventGridService
    {
        #region Fields

        private readonly ITranslationsService _translationsService;
        private readonly IConsoleLoggerService _consoleLoggerService;

        private EventGridClient _eventGridClient;

        private string _endpoint;
        private string _key;
        private string _topic;
        private string _subject;
        private string _eventType;
        private string _dataVersion;

        private bool _isConnected;

        #endregion

        #region Constructors

        public EventGridService(ITranslationsService translationsService,
                                IConsoleLoggerService consoleLoggerService)
        {
            _consoleLoggerService = consoleLoggerService;
            _translationsService = translationsService;
        }

        #endregion

        #region IEventGridService

        public bool IsConnected => _isConnected;

        public Task Connect(string endpoint,
                            string key,
                            string topic,
                            string subject,
                            string eventType,
                            string dataVersion)
        {
            if (string.IsNullOrEmpty(endpoint)
             || string.IsNullOrEmpty(key)
             || string.IsNullOrEmpty(topic)
             || string.IsNullOrEmpty(subject)
             || string.IsNullOrEmpty(eventType)
             || string.IsNullOrEmpty(dataVersion))
            {
                _consoleLoggerService.Log(value: _translationsService.GetString("ParametersNotValid"),
                                          logType: Types.EventGrid);
                return Task.FromResult(true);
            }

            _endpoint = endpoint;
            _key = key;
            _topic = topic;
            _subject = subject;
            _eventType = eventType;
            _dataVersion = dataVersion;

            TopicCredentials topicCredentials = new TopicCredentials(_key);
            _eventGridClient = new EventGridClient(topicCredentials);
            _isConnected = true;

            _consoleLoggerService.Log(value: _translationsService.GetString("EventGridConnected"),
                                      logType: Types.EventGrid);

            return Task.FromResult(true);
        }

        public Task Disconnect()
        {
            _isConnected = false;
            _endpoint = null;
            _key = null;
            _topic = null;
            _subject = null;
            _eventType = null;
            _dataVersion = null;

            _eventGridClient.Dispose();
            _eventGridClient = null;

            _consoleLoggerService.Log(value: _translationsService.GetString("EventGridDisconnected"),
                                      logType: Types.EventGrid);
            return Task.FromResult(true);
        }

        public async Task SendRequest(string request)
        {
            dynamic obj = JsonConvert.DeserializeObject(request);
            var eventGridEvent = new EventGridEvent()
            {
                Id = Guid.NewGuid().ToString(),
                EventType = _eventType,
                Topic = _topic,
                Data = obj,
                EventTime = DateTime.Now,
                Subject = _subject,
                DataVersion = _dataVersion
            };
            var domainHostname = new Uri(_endpoint).Host;

            await _eventGridClient.PublishEventsAsync(domainHostname, new List<EventGridEvent>() { eventGridEvent });

            var logMessage = string.Format(_translationsService.GetString("EventSent"),
                                           Environment.NewLine,
                                           request,
                                           Environment.NewLine);
            _consoleLoggerService.Log(value: logMessage,
                                      logType: Types.EventGrid);
        }

        #endregion
    }
}
