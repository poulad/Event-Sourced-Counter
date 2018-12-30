using System;
using System.Threading;
using System.Threading.Tasks;
using ESC.Data;
using ESC.EventHandlers.Identicon.Extensions;
using ESC.EventHandlers.Identicon.Handlers;
using ESC.EventHandlers.Identicon.Subscriptions;
using ESC.Events;
using EventStore.ClientAPI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ESC.EventHandlers.Identicon
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

            services.AddSingleton<IdenticonSubscriber>();
            services.AddTransient<NewIdenticonHandler>();
        }

        public async Task RunAsync(
            IServiceProvider serviceProvider
        )
        {
            var cancellationTokenSource = new CancellationTokenSource();

            using (var scope = serviceProvider.CreateScope())
            {
                scope.CreateMongoDbSchema();

                // "$et-counter.new.created" stream
                {
                    var esClient = scope.ServiceProvider.GetRequiredService<IEventStoreConnection>();
                    var configRepo = scope.ServiceProvider.GetRequiredService<IConfigRepository>();
                    var subscriber = scope.ServiceProvider.GetRequiredService<IdenticonSubscriber>();

                    await esClient.ConnectAsync()
                        .ConfigureAwait(false);

                    string stream = StreamNames.GetStreamNameForEventType(Types.NewCounterCreated);
                    long? lastPosition = await configRepo
                        .GetLastProcessedEventIdAsync(stream)
                        .ConfigureAwait(false);

                    esClient.SubscribeToStreamFrom(
                        stream,
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
