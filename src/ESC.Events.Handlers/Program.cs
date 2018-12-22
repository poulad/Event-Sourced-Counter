using System;
using System.Text;
using System.Threading.Tasks;
using ESC.Data.Redis;
using ESC.Data.Redis.Entities;
using EventStore.ClientAPI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ESC.Events.Handlers
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
                await eventStoreClient.ConnectAsync()
                    .ConfigureAwait(false);

                string lastPosition = await _repo.GetLastEventPositionAsync()
                    .ConfigureAwait(false);

                if (lastPosition != null)
                {
                    // ToDo catch up first
                }

                await eventStoreClient.SubscribeToAllAsync(false, EventAppeared)
                    .ConfigureAwait(false);

                Console.WriteLine("Subscription Activated.");
                await Task.Delay(TimeSpan.FromHours(2))
                    .ConfigureAwait(false);
                Console.WriteLine("BYE!");
            }
        }

        private static Task EventAppeared(EventStoreSubscription subscription, ResolvedEvent resolvedEvent)
        {
            if (!resolvedEvent.OriginalStreamId.StartsWith(StreamNames.CounterStreamPrefix))
            {
                return Task.CompletedTask;
            }

            Task task;
            switch (resolvedEvent.Event.EventType)
            {
                case Types.CounterCreated:
                    task = CreateCounterAsync(resolvedEvent.Event);
                    break;
                case Types.CounterIncremented:
                    task = IncrementCounterAsync(resolvedEvent.Event);
                    break;
                default:
                    string json = JsonConvert.SerializeObject(resolvedEvent, Formatting.Indented);
                    Console.WriteLine($"Invalid event found!{Environment.NewLine}{json}");
                    task = Task.CompletedTask;
                    break;
            }

            return task;
        }

        static async Task CreateCounterAsync(RecordedEvent recordedEvent)
        {
            var counter = new Counter
            {
                Name = recordedEvent.EventStreamId.Substring("counter:".Length),
                CreatedAt = recordedEvent.Created,
            };

            await _repo.SetCounterAsync(counter)
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

            var counter = await _repo.GetCounterByNameAsync(counterName)
                .ConfigureAwait(false);

            counter.Value += count;

            await _repo.SetCounterAsync(counter)
                .ConfigureAwait(false);
        }
    }
}