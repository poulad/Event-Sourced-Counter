using System;
using System.Text;
using System.Threading.Tasks;
using ESC.Events;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace ESC.Web.Services
{
    public class EventStoreClient
    {
        const string ConnectionString = "ConnectTo=tcp://admin:changeit@localhost:1113; HeartBeatTimeout=500";

        public async Task AppendMutationRequestEventAsync(IEvent e, bool serializeEvent = true)
        {
            const string streamName = StreamNames.CounterMutationsStreamName;

            using (var conn = EventStoreConnection.Create(ConnectionString))
            {
                await conn.ConnectAsync()
                    .ConfigureAwait(false);

                Task appendTask;
                if (serializeEvent)
                {
                    string json = JsonConvert.SerializeObject(e);
                    byte[] bytes = Encoding.UTF8.GetBytes(json);

                    appendTask = conn.AppendToStreamAsync(
                        streamName,
                        ExpectedVersion.Any,
                        new EventData(Guid.NewGuid(), e.Type, true, bytes, null)
                    );
                }
                else
                {
                    appendTask = conn.AppendToStreamAsync(
                        streamName,
                        ExpectedVersion.Any,
                        new EventData(Guid.NewGuid(), e.Type, false, default, default)
                    );
                }

                await appendTask.ConfigureAwait(false);
            }
        }
    }
}
