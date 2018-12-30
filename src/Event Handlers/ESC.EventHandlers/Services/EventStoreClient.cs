using System;
using System.Text;
using System.Threading.Tasks;
using ESC.Events;
using EventStore.ClientAPI;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ESC.EventHandlers.Services
{
    public class EventStoreClient : IEventStoreClient
    {
        private readonly IEventStoreConnection _connection;
        private readonly ILogger<EventStoreClient> _logger;
        private bool _isConnected;

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
            await EnsureConnectedAsync()
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

        private async Task EnsureConnectedAsync()
        {
            if (!_isConnected)
            {
                await _connection.ConnectAsync()
                    .ConfigureAwait(false);

                _isConnected = true;
            }
        }
    }
}
