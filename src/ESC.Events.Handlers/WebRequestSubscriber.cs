using System;
using System.Text;
using System.Threading.Tasks;
using ESC.Data.Redis;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace ESC.Events.Handlers
{
    public class WebRequestSubscriber : IStreamCatchUpSubscriber
    {
        private readonly CounterRepo _counterRepo;

        public WebRequestSubscriber(
            CounterRepo counterRepo
        )
        {
            _counterRepo = counterRepo;
        }

        public async Task OnCatchUpSubscriptionEvent(
            EventStoreCatchUpSubscription subscription,
            ResolvedEvent resolvedEvent
        )
        {
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
                    string eventData = Encoding.UTF8.GetString(resolvedEvent.OriginalEvent.Data);
                    Console.WriteLine(
                        $"Invalid event found!{Environment.NewLine}{eventData}{Environment.NewLine}{json}"
                    );
                    break;
            }

            long lastProcessedEventId = resolvedEvent.OriginalEventNumber;

            await _counterRepo.SetLastProcessedEventIdAsync(
                StreamNames.WebRequestStreamName,
                lastProcessedEventId
            ).ConfigureAwait(false);

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"Subscription {subscription.StreamId} processed {lastProcessedEventId}.");
            Console.ResetColor();
            Console.WriteLine();
        }

        public void OnLiveProcessingStarted(
            EventStoreCatchUpSubscription subscription
        )
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write($"Subscription {subscription.StreamId} has gone live!");
            Console.ResetColor();
            Console.WriteLine();
        }

        public void OnCatchUpSubscriptionDropped(
            EventStoreCatchUpSubscription subscription,
            SubscriptionDropReason dropReason,
            Exception exception
        )
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(
                $"Subscription {subscription.StreamId} dropped! {dropReason}{Environment.NewLine}{exception}"
            );
            Console.ResetColor();
        }
    }
}