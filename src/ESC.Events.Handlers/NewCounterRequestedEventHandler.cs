using System.Threading.Tasks;
using ESC.Data;
using ESC.Data.Entities;
using ESC.Events.Handlers.Services;
using EventStore.ClientAPI;
using Microsoft.Extensions.Logging;
using NUlid;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace ESC.Events.Handlers
{
    public class NewCounterRequestedEventHandler : IEventHandler<NewCounterRequestedEvent>
    {
        private readonly IEventStoreClient _esClient;
        private readonly ICounterRepository _counterRepo;
        private readonly ILogger _logger;

        public NewCounterRequestedEventHandler(
            IEventStoreClient esClient,
            ICounterRepository counterRepo,
            ILogger<NewCounterRequestedEventHandler> logger
        )
        {
            _esClient = esClient;
            _counterRepo = counterRepo;
            _logger = logger;
        }

        public async Task Handle(
            ResolvedEvent _,
            NewCounterRequestedEvent e
        )
        {
            // create new counter
            var id = Ulid.NewUlid();
            var newCounter = new Counter
            {
                Id = id.ToString(),
                Name = e.Name,
                Count = 0,
                CreatedAt = id.Time.UtcDateTime,
                Version = 0,
            };
            var error = await _counterRepo.AddAsync(newCounter)
                .ConfigureAwait(false);

            if (error is null)
            {
                await _esClient.AppendEventAsync(
                    StreamNames.GetStreamNameFromCounterId(newCounter.Id),
                    new NewCounterCreatedEvent
                    {
                        Counter = newCounter,
                        CorrelationId = e.CorrelationId,
                    }
                ).ConfigureAwait(false);
            }
            else if (error.Code == ErrorCodes.Duplicate)
            {
                // ToDo raise an validation event and push it to the client using correlation ID provided
            }
            else
            {
                // ToDo raise an validation event and push it to the client using correlation ID provided
            }
        }
    }
}
