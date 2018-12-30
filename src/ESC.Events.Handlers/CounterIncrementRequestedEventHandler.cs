using System.Threading.Tasks;
using ESC.Data;
using ESC.Events.Handlers.Services;
using EventStore.ClientAPI;
using Microsoft.Extensions.Logging;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace ESC.Events.Handlers
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

        public async Task Handle(
            ResolvedEvent _,
            CounterIncrementRequestedEvent e
        )
        {
            var result = await _counterRepo.GetByNameAsync(e.CounterName)
                .ConfigureAwait(false);

            if (result.Error is null)
            {
                var counter = result.Counter;
                counter.Count += e.By;
                counter.Version++;

                var addError = await _counterRepo.AddAsync(counter)
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
