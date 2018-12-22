using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESC.Data.Redis;
using ESC.Data.Redis.Entities;
using EventStore.ClientAPI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ESC.ModelAggregator.Redis
{
    class Program
    {
        const string EventStoreConnectionString = "ConnectTo=tcp://admin:changeit@localhost:1113; HeartBeatTimeout=500";

        private static CounterRepo _repo;

        static async Task Main()
        {
            using (_repo = new CounterRepo())
            using (var eventStoreClient = EventStoreConnection.Create(EventStoreConnectionString))
            {
                await eventStoreClient.ConnectAsync();

                Position startPosition = Position.Start;
                AllEventsSlice eventsSlice;
                do
                {
                    eventsSlice = await eventStoreClient
                        .ReadAllEventsForwardAsync(startPosition, 1000, false)
                        .ConfigureAwait(false);

                    var events = eventsSlice.Events
                        .Where(ev => ev.OriginalStreamId.StartsWith("counter:"))
                        .Select(ev => ev.Event);

                    foreach (var recordedEvent in events)
                    {
                        await HandleAsync(recordedEvent)
                            .ConfigureAwait(false);
                    }

                    startPosition = eventsSlice.NextPosition;
                } while (!eventsSlice.IsEndOfStream);
            }
        }

        static async Task HandleAsync(RecordedEvent recordedEvent)
        {
            switch (recordedEvent.EventType)
            {
                case Events.Types.CounterCreated:
                    await CreateCounterAsync(recordedEvent)
                        .ConfigureAwait(false);
                    break;
                case Events.Types.CounterIncremented:
                    await IncrementCounterAsync(recordedEvent)
                        .ConfigureAwait(false);
                    break;
                default:
                    string json = JsonConvert.SerializeObject(recordedEvent, Formatting.Indented);
                    Console.WriteLine($"Invalid event found!{Environment.NewLine}{json}");
                    break;
            }
        }

        static async Task CreateCounterAsync(RecordedEvent recordedEvent)
        {
            var counter = new Counter
            {
                Name = recordedEvent.EventStreamId.Substring("counter:".Length),
                CreatedAt = recordedEvent.Created,
            };

            await _repo.SetCounter(counter)
                .ConfigureAwait(false);
        }

        static async Task IncrementCounterAsync(RecordedEvent recordedEvent)
        {
            int count;
            {
                string evJson = Encoding.UTF8.GetString(recordedEvent.Data);
                count = JsonConvert.DeserializeObject<JObject>(evJson).Value<int>("count");
            }

            string counterName = recordedEvent.EventStreamId.Substring("counter:".Length);

            var counter = await _repo.GetCounterByName(counterName)
                .ConfigureAwait(false);

            counter.Value += count;

            await _repo.SetCounter(counter)
                .ConfigureAwait(false);
        }
    }
}