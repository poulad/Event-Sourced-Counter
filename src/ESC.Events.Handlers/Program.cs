using System;
using System.Threading;
using System.Threading.Tasks;
using ESC.Data.Redis;
using EventStore.ClientAPI;

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

                // "counterMutations" stream
                {
                    long? lastPosition = await _repo
                        .GetLastProcessedEventIdAsync(StreamNames.CounterMutationsStreamName)
                        .ConfigureAwait(false);

                    var subscriber = new CounterMutationsSubscriber(_repo);
                    esClient.SubscribeToStreamFrom(
                        StreamNames.CounterMutationsStreamName,
                        lastPosition,
                        CatchUpSubscriptionSettings.Default,
                        subscriber.OnCatchUpSubscriptionEvent,
                        subscriber.OnLiveProcessingStarted,
                        (subscription, reason, exception) =>
                        {
                            subscriber.OnCatchUpSubscriptionDropped(subscription, reason, exception);
                            cancellationSource.Cancel();
                        }
                    );
                }

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
    }
}
