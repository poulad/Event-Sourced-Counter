using System;
using System.Threading.Tasks;
using ESC.Data.Redis;
using ESC.Data.Redis.Entities;
using EventStore.ClientAPI;

namespace ESC.Events.Handlers
{
    public class NewCounterRequestedEventHandler : IEventHandler<NewCounterRequestedEvent>
    {
        private readonly CounterRepo _counterRepo;

        public NewCounterRequestedEventHandler(
            CounterRepo counterRepo
        )
        {
            _counterRepo = counterRepo;
        }

        public async Task Handle(
            ResolvedEvent _,
            NewCounterRequestedEvent e
        )
        {
            bool isDuplicateCounterName;
            {
                var existingCounter = await _counterRepo.GetCounterByNameAsync(e.Name)
                    .ConfigureAwait(false);
                isDuplicateCounterName = existingCounter != null;
            }

            if (!isDuplicateCounterName)
            {
                // create new counter
                var newCounter = new Counter
                {
                    Id = Guid.NewGuid().ToString(), // ToDo How to ensure this ID is unique?
                    Name = e.Name,
                    Count = 0,
                    CreatedAt = DateTime.UtcNow,
                    Version = 0,
                };
                await _counterRepo.SetCounterAsync(newCounter)
                    .ConfigureAwait(false);

                // emit the event
                await EventStoreClient.AppendEventAsync(
                    StreamNames.GetStreamNameFromCounterId(newCounter.Id),
                    new NewCounterCreatedEvent
                    {
                        Name = newCounter.Name,
                        CorrelationId = e.CorrelationId,
                    }
                ).ConfigureAwait(false);
            }
            else
            {
                // ToDo raise an validation event and push it to the client using correlation ID provided
            }
        }
    }
}