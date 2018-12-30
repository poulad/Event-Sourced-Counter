using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ESC.Data;
using ESC.EventHandlers.Abstractions;
using ESC.EventHandlers.Identicon.Handlers;
using ESC.Events;
using EventStore.ClientAPI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace ESC.EventHandlers.Identicon.Subscriptions
{
    /// <summary>
    /// Subscription to create identicons for new counters
    /// </summary>
    public class IdenticonSubscriber : IStreamCatchUpSubscriber
    {
        private readonly IConfigRepository _configRepo;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;

        public IdenticonSubscriber(
            IConfigRepository configRepo,
            IServiceProvider serviceProvider,
            ILogger<IdenticonSubscriber> logger
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
            string eventData = Encoding.UTF8.GetString(resolvedEvent.Event.Data);

            switch (resolvedEvent.Event.EventType)
            {
                case Types.NewCounterCreated:
                    var handler = _serviceProvider.GetRequiredService<NewIdenticonHandler>();

                    var e = JsonConvert.DeserializeObject<NewCounterCreatedEvent>(eventData);
                    await handler.HandleAsync(resolvedEvent, e, CancellationToken.None)
                        .ConfigureAwait(false);
                    break;

                default:
                    string json = JsonConvert.SerializeObject(resolvedEvent, Formatting.Indented);
                    _logger.LogWarning(
                        "Invalid event found in stream {streamId}.\n{eventData}\n{json}",
                        subscription.StreamId, eventData, json
                    );
                    return;
            }

            long lastProcessedEventId = resolvedEvent.OriginalEventNumber;

            await _configRepo.SetLastProcessedEventIdAsync(
                subscription.StreamId,
                lastProcessedEventId
            ).ConfigureAwait(false);

            _logger.LogDebug(
                "Subscription for stream {streamId} processed {eventType} event number {lastProcessedEventId}.",
                subscription.StreamId, resolvedEvent.Event.EventType, lastProcessedEventId
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
