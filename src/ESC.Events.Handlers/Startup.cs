using System;
using System.Threading;
using System.Threading.Tasks;
using ESC.Data;
using ESC.Events.Handlers.Extensions;
using EventStore.ClientAPI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ESC.Events.Handlers
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
            using (var scope = serviceProvider.CreateScope())
            {
                scope.CreateMongoDbSchema();

                var esClient = scope.ServiceProvider.GetRequiredService<IEventStoreConnection>();
                var configRepo = scope.ServiceProvider.GetRequiredService<IConfigRepository>();

                await esClient.ConnectAsync()
                    .ConfigureAwait(false);

                var cancellationTokenSource = new CancellationTokenSource();

                // "counterMutations" stream
                {
                    long? lastPosition = await configRepo
                        .GetLastProcessedEventIdAsync(StreamNames.CounterMutationsStreamName)
                        .ConfigureAwait(false);

                    var subscriber = scope.ServiceProvider.GetRequiredService<CounterMutationsSubscriber>();
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
