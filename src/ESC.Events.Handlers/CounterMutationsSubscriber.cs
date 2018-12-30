using System;
using System.Text;
using System.Threading.Tasks;
using ESC.Data;
using EventStore.ClientAPI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace ESC.Events.Handlers
{
    public class CounterMutationsSubscriber : IStreamCatchUpSubscriber
    {
        private readonly IConfigRepository _configRepo;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;

        public CounterMutationsSubscriber(
            IConfigRepository configRepo,
            IServiceProvider serviceProvider,
            ILogger<CounterMutationsSubscriber> logger
        )
        {
            _configRepo = configRepo;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task OnCatchUpSubscriptionEvent(
            EventStoreCatchUpSubscription subscription,
            ResolvedEvent resolvedEvent
        )
        {
            switch (resolvedEvent.Event.EventType)
            {
                case Types.NewCounterRequested:
                    var handler1 = _serviceProvider.GetRequiredService<NewCounterRequestedEventHandler>();

                    string data1 = Encoding.UTF8.GetString(resolvedEvent.OriginalEvent.Data);
                    var e1 = JsonConvert.DeserializeObject<NewCounterRequestedEvent>(data1);
                    handler1.Handle(resolvedEvent, e1)
                        .ConfigureAwait(false);
                    break;

                case Types.CounterIncrementRequested:
                    var handler2 = _serviceProvider.GetRequiredService<CounterIncrementRequestedEventHandler>();

                    string data2 = Encoding.UTF8.GetString(resolvedEvent.OriginalEvent.Data);
                    var e = JsonConvert.DeserializeObject<CounterIncrementRequestedEvent>(data2);
                    handler2.Handle(resolvedEvent, e)
                        .ConfigureAwait(false);
                    break;

                default:
                    string json = JsonConvert.SerializeObject(resolvedEvent, Formatting.Indented);
                    string eventData = Encoding.UTF8.GetString(resolvedEvent.OriginalEvent.Data);
                    _logger.LogWarning(
                        "Invalid event found in stream {streamId}.\n{eventData}\n{json}",
                        subscription.StreamId, eventData, json
                    );
                    return;
            }

            long lastProcessedEventId = resolvedEvent.OriginalEventNumber;

            await _configRepo.SetLastProcessedEventIdAsync(
                StreamNames.CounterMutationsStreamName,
                lastProcessedEventId
            ).ConfigureAwait(false);

            _logger.LogDebug(
                "Subscription for stream {streamId} processed event number {lastProcessedEventId}.",
                subscription.StreamId, lastProcessedEventId
            );
        }

        public void OnLiveProcessingStarted(
            EventStoreCatchUpSubscription subscription
        )
        {
            _logger.LogInformation("Subscription for stream {streamId} has gone live.", subscription.StreamId);
        }

        public void OnCatchUpSubscriptionDropped(
            EventStoreCatchUpSubscription subscription,
            SubscriptionDropReason dropReason,
            Exception exception
        )
        {
            _logger.LogCritical(
                exception,
                "Subscription for stream {streamId} dropped due to {dropReason}.",
                subscription.StreamId, dropReason
            );
        }
    }
}
