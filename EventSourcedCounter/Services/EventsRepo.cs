using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventSourcedCounter.Events;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace EventSourcedCounter.Services
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

        public async Task<IReadOnlyCollection<RecordedEvent>> GetAllCounterEventsAsync(string counterName)
        {
            string streamName = $"counter:{counterName}";
            var recordedEvents = new List<RecordedEvent>();

            using (var conn = EventStoreConnection.Create(ConnectionString))
            {
                await conn.ConnectAsync();

                long startPosition = 0;
                StreamEventsSlice eventsSlice;
                do
                {
                    eventsSlice = await conn
                        .ReadStreamEventsForwardAsync(streamName, startPosition, 10, true);

                    recordedEvents.AddRange(eventsSlice.Events.Select(e => e.Event));

                    startPosition = eventsSlice.NextEventNumber;
                } while (!eventsSlice.IsEndOfStream);
            }

            return recordedEvents.AsReadOnly();
        }
    }
}