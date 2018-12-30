using System;
using System.Threading;
using System.Threading.Tasks;
using ESC.Data;
using ESC.Data.Entities;
using ESC.EventHandlers.Abstractions;
using ESC.EventHandlers.Services;
using ESC.Events;
using EventStore.ClientAPI;
using Microsoft.Extensions.Logging;
using NUlid;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace ESC.EventHandlers.CounterMutations.Handlers
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

        public async Task HandleAsync(
            ResolvedEvent _,
            NewCounterRequestedEvent e,
            CancellationToken cancellationToken
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
            var error = await _counterRepo.AddAsync(newCounter, cancellationToken)
                .ConfigureAwait(false);

            if (error is null)
            {
                try
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
                catch (Exception exception)
                {
                    // ToDo
                    throw;
                }
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
