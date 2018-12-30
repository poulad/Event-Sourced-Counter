using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESC.Events;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace ESC.Web.Services
{
    public class EventStoreClient : IEventStoreClient
    {
        private readonly IEventStoreConnection _connection;

        public EventStoreClient(
            IEventStoreConnection connection
        )
        {
            _connection = connection;
        }

        public async Task AppendMutationRequestEventAsync(
            IEvent e,
            bool serializeEvent = true,
            CancellationToken cancellationToken = default
        )
        {
            const string streamName = StreamNames.CounterMutationsStreamName;

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
