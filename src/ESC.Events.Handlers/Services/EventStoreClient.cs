using System;
using System.Text;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ESC.Events.Handlers.Services
{
    public class EventStoreClient : IEventStoreClient
    {
        private readonly IEventStoreConnection _connection;
        private readonly ILogger<EventStoreClient> _logger;

        public EventStoreClient(
            IEventStoreConnection connection,
            ILogger<EventStoreClient> logger
        )
        {
            _connection = connection;
            _logger = logger;
        }

        public async Task AppendEventAsync(
            string streamName,
            IEvent e,
            bool serializeEvent = true
        )
        {
            await _connection.ConnectAsync()
                .ConfigureAwait(false);

            Task appendTask;
            if (serializeEvent)
            {
                string json = JsonConvert.SerializeObject(e);
                byte[] bytes = Encoding.UTF8.GetBytes(json);

                appendTask = _connection.AppendToStreamAsync(
                    streamName,
                    ExpectedVersion.Any,
                    new EventData(Guid.NewGuid(), e.Type, true, bytes, null)
                );
            }
            else
            {
                appendTask = _connection.AppendToStreamAsync(
                    streamName,
                    ExpectedVersion.Any,
                    new EventData(Guid.NewGuid(), e.Type, false, default, default)
                );
            }

            await appendTask.ConfigureAwait(false);
        }
    }
}
