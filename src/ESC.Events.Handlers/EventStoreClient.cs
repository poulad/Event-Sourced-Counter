using System;
using System.Text;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace ESC.Events.Handlers
{
    public static class EventStoreClient
    {
        public static async Task AppendEventAsync(
            string streamName,
            IEvent e,
            bool serializeEvent = true
        )
        {
            using (var conn = EventStoreConnection.Create(Program.EventStoreConnectionString))
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