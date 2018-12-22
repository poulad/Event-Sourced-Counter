using System;
using System.Text;
using System.Threading.Tasks;
using ESC.Web.Events;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace ESC.Web.Services
{
    public class EventsRepo
    {
        const string ConnectionString = "ConnectTo=tcp://admin:changeit@localhost:1113; HeartBeatTimeout=500";

        public async Task AppendCounterEventAsync(string counterName, IEvent ev)
        {
            string streamName = $"counter:{counterName}";

            using (var conn = EventStoreConnection.Create(ConnectionString))
            {
                await conn.ConnectAsync();

                if (ev is SimpleEvent)
                {
                    await conn.AppendToStreamAsync(
                        streamName,
                        ExpectedVersion.Any,
                        new EventData(Guid.NewGuid(), ev.Type, false, null, null)
                    );
                }
                else
                {
                    string json = JsonConvert.SerializeObject(ev);
                    byte[] bytes = Encoding.UTF8.GetBytes(json);

                    await conn.AppendToStreamAsync(
                        streamName,
                        ExpectedVersion.Any,
                        new EventData(Guid.NewGuid(), ev.Type, true, bytes, null)
                    );
                }
            }
        }
    }
}