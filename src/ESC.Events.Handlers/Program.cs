using System;
using System.Text;
using System.Threading;
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
        internal const string EventStoreConnectionString =
            "ConnectTo=tcp://admin:changeit@localhost:1113; HeartBeatTimeout=500";

        private static CounterRepo _repo;

        static async Task Main()
        {
            using (_repo = new CounterRepo())
            using (var esClient = EventStoreConnection.Create(EventStoreConnectionString))
            {
                await esClient.ConnectAsync()
                    .ConfigureAwait(false);

                var cancellationSource = new CancellationTokenSource();

                // "WebRequest" stream
                {
                    long? lastPosition = await _repo.GetLastProcessedEventIdAsync(StreamNames.WebRequestStreamName)
                        .ConfigureAwait(false);

                    var webRequestSubscriber = new WebRequestSubscriber(_repo);
                    esClient.SubscribeToStreamFrom(
                        StreamNames.WebRequestStreamName,
                        lastPosition,
                        CatchUpSubscriptionSettings.Default,
                        webRequestSubscriber.OnCatchUpSubscriptionEvent,
                        webRequestSubscriber.OnLiveProcessingStarted,
                        (subscription, reason, exception) =>
                        {
                            webRequestSubscriber.OnCatchUpSubscriptionDropped(subscription, reason, exception);
                            cancellationSource.Cancel();
                        }
                    );
                }

//                esClient.SubscribeToStreamFrom(
//                    "$ce-counter",
//                    lastPosition,
//                    CatchUpSubscriptionSettings.Default,
//                    EventAppeared
//                );

                try
                {
                    await Task.Delay(TimeSpan.FromHours(2), cancellationSource.Token)
                        .ConfigureAwait(false);

                    Console.WriteLine("2 hours passed! Find a better way to do this.");
                }
                catch (TaskCanceledException)
                {
                    Console.WriteLine("Whoops! Something went wrong...");
                }
            }
        }

        private static Task EventAppeared(EventStoreCatchUpSubscription subscription, ResolvedEvent resolvedEvent)
        {
            if (!resolvedEvent.OriginalStreamId.StartsWith(StreamNames.CounterStreamPrefix))
            {
                return Task.CompletedTask;
            }

            Task task;
            switch (resolvedEvent.Event.EventType)
            {
//                case Types.CounterStarted:
//                    task = CreateCounterAsync(resolvedEvent.Event);
//                    break;
//                case Types.CounterIncremented:
//                    task = IncrementCounterAsync(resolvedEvent.Event);
//                    break;
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