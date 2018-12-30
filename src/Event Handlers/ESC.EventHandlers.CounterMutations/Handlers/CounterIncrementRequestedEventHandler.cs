using System.Threading;
using System.Threading.Tasks;
using ESC.Data;
using ESC.EventHandlers.Abstractions;
using ESC.EventHandlers.Services;
using ESC.Events;
using EventStore.ClientAPI;
using Microsoft.Extensions.Logging;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace ESC.EventHandlers.CounterMutations.Handlers
{
    public class CounterIncrementRequestedEventHandler : IEventHandler<CounterIncrementRequestedEvent>
    {
        private readonly IEventStoreClient _esClient;
        private readonly ICounterRepository _counterRepo;
        private readonly ILogger _logger;

        public CounterIncrementRequestedEventHandler(
            IEventStoreClient esClient,
            ICounterRepository counterRepo,
            ILogger<CounterIncrementRequestedEventHandler> logger
        )
        {
            _esClient = esClient;
            _counterRepo = counterRepo;
            _logger = logger;
        }

        public async Task HandleAsync(
            ResolvedEvent _,
            CounterIncrementRequestedEvent e,
            CancellationToken cancellationToken
        )
        {
            var result = await _counterRepo.GetByNameAsync(e.CounterName, cancellationToken)
                .ConfigureAwait(false);

            if (result.Error is null)
            {
                var counter = result.Counter;
                counter.Count += e.By;
                counter.Version++;

                var addError = await _counterRepo.AddAsync(counter, cancellationToken)
                    .ConfigureAwait(false);

                if (addError is null)
                {
                    // emit the event
                    await _esClient.AppendEventAsync(
                        StreamNames.GetStreamNameFromCounterId(counter.Id),
                        new CounterIncrementedEvent
                        {
                            By = e.By,
                            CounterName = counter.Name,
                            NewVersion = counter.Version,
                            CorrelationId = e.CorrelationId,
                        }
                    ).ConfigureAwait(false);
                }
                else
                {
                    // ToDo Counter is deleted meanwhile! Emit an error and use the correlation ID.
                }
            }
            else
            {
                // ToDo Counter is deleted meanwhile! Emit an error and use the correlation ID.
            }
        }
    }
}
