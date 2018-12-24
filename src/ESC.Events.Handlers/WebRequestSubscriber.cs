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
                case Types.NewCounterRequested:
                    var handler = new NewCounterRequestedEventHandler(_counterRepo);

                    string data = Encoding.UTF8.GetString(resolvedEvent.OriginalEvent.Data);
                    var e = JsonConvert.DeserializeObject<NewCounterRequestedEvent>(data);
                    handler.Handle(resolvedEvent, e)
                        .ConfigureAwait(false);
                    break;
                default:
                    string json = JsonConvert.SerializeObject(resolvedEvent, Formatting.Indented);
                    string eventData = Encoding.UTF8.GetString(resolvedEvent.OriginalEvent.Data);
                    Console.WriteLine(
                        $"Invalid event found in stream `{subscription.StreamId}`!" + Environment.NewLine +
                        eventData + Environment.NewLine + json
                    );
                    return;
            }

            long lastProcessedEventId = resolvedEvent.OriginalEventNumber;

            await _counterRepo.SetLastProcessedEventIdAsync(
                StreamNames.WebRequestStreamName,
                lastProcessedEventId
            ).ConfigureAwait(false);

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(
                $"Subscription for stream `{subscription.StreamId}` processed " +
                $"event number {lastProcessedEventId}."
            );
            Console.ResetColor();
            Console.WriteLine();
        }

        public void OnLiveProcessingStarted(
            EventStoreCatchUpSubscription subscription
        )
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write($"Subscription for stream `{subscription.StreamId}` has gone live!");
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
                $"Subscription for stream `{subscription.StreamId}` dropped! {dropReason}" +
                Environment.NewLine + exception
            );
            Console.ResetColor();
        }
    }
}