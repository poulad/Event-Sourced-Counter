using System.Threading.Tasks;
using ESC.Data.Redis;
using EventStore.ClientAPI;

namespace ESC.Events.Handlers
{
    public class CounterIncrementRequestedEventHandler : IEventHandler<CounterIncrementRequestedEvent>
    {
        private readonly CounterRepo _counterRepo;

        public CounterIncrementRequestedEventHandler(
            CounterRepo counterRepo
        )
        {
            _counterRepo = counterRepo;
        }

        public async Task Handle(
            ResolvedEvent _,
            CounterIncrementRequestedEvent e
        )
        {
            var counterEntity = await _counterRepo.GetCounterByNameAsync(e.CounterName)
                .ConfigureAwait(false);

            if (counterEntity != null)
            {
                counterEntity.Count += e.By;
                counterEntity.Version++;

                await _counterRepo.SetCounterAsync(counterEntity)
                    .ConfigureAwait(false);

                // emit the event
                await EventStoreClient.AppendEventAsync(
                    StreamNames.GetStreamNameFromCounterId(counterEntity.Id),
                    new CounterIncrementedEvent
                    {
                        By = e.By,
                        CounterName = counterEntity.Name,
                        NewVersion = counterEntity.Version,
                        CorrelationId = e.CorrelationId,
                    }
                ).ConfigureAwait(false);
            }
            else
            {
                // ToDo Counter is deleted meanwhile! Emit an error and use the correlation ID.
            }
        }
    }
}
