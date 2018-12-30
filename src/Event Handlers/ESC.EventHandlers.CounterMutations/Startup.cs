using System;
using System.Threading;
using System.Threading.Tasks;
using ESC.Data;
using ESC.EventHandlers.CounterMutations.Extensions;
using ESC.EventHandlers.CounterMutations.Handlers;
using ESC.EventHandlers.CounterMutations.Subscriptions;
using ESC.Events;
using EventStore.ClientAPI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ESC.EventHandlers.CounterMutations
{
    public class Startup
    {
        private IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMongoDb(Configuration.GetSection("Data"));
            services.AddEventStore(Configuration.GetSection("Data"));

            services.AddSingleton<CounterMutationsSubscriber>();
            services.AddTransient<NewCounterRequestedEventHandler>();
            services.AddTransient<CounterIncrementRequestedEventHandler>();
        }

        public async Task RunAsync(
            IServiceProvider serviceProvider
        )
        {
            var cancellationTokenSource = new CancellationTokenSource();

            using (var scope = serviceProvider.CreateScope())
            {
                scope.CreateMongoDbSchema();

                // "counterMutations" stream
                {
                    var esClient = scope.ServiceProvider.GetRequiredService<IEventStoreConnection>();
                    var configRepo = scope.ServiceProvider.GetRequiredService<IConfigRepository>();
                    var subscriber = scope.ServiceProvider.GetRequiredService<CounterMutationsSubscriber>();

                    await esClient.ConnectAsync()
                        .ConfigureAwait(false);

                    long? lastPosition = await configRepo
                        .GetLastProcessedEventIdAsync(StreamNames.CounterMutationsStreamName)
                        .ConfigureAwait(false);

                    esClient.SubscribeToStreamFrom(
                        StreamNames.CounterMutationsStreamName,
                        lastPosition,
                        CatchUpSubscriptionSettings.Default,
                        subscriber.OnCatchUpSubscriptionEvent,
                        subscriber.OnLiveProcessingStarted,
                        (subscription, reason, exception) =>
                        {
                            subscriber.OnCatchUpSubscriptionDropped(subscription, reason, exception);
                            cancellationTokenSource.Cancel();
                        }
                    );
                }

                while (!cancellationTokenSource.IsCancellationRequested)
                {
                    try
                    {
                        await Task.Delay(int.MaxValue - 1, cancellationTokenSource.Token)
                            .ConfigureAwait(false);
                    }
                    catch (TaskCanceledException) { }
                }
            }
        }
    }
}
