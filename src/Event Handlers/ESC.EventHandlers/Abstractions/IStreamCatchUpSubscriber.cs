using System;
using System.Threading.Tasks;
using EventStore.ClientAPI;

namespace ESC.EventHandlers.Abstractions
{
    public interface IStreamCatchUpSubscriber
    {
        Task OnCatchUpSubscriptionEvent(
            EventStoreCatchUpSubscription subscription,
            ResolvedEvent resolvedEvent
        );

        void OnLiveProcessingStarted(
            EventStoreCatchUpSubscription subscription
        );

        void OnCatchUpSubscriptionDropped(
            EventStoreCatchUpSubscription subscription,
            SubscriptionDropReason dropReason,
            Exception exception
        );
    }
}
