using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ESC.Data;
using ESC.EventHandlers.Abstractions;
using ESC.EventHandlers.Services;
using ESC.Events;
using EventStore.ClientAPI;
using Microsoft.Extensions.Logging;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace ESC.EventHandlers.Identicon.Handlers
{
    public class NewIdenticonHandler : IEventHandler<NewCounterCreatedEvent>
    {
        private readonly IEventStoreClient _esClient;
        private readonly ICounterRepository _counterRepo;
        private readonly ILogger _logger;

        public NewIdenticonHandler(
            IEventStoreClient esClient,
            ICounterRepository counterRepo,
            ILogger<NewIdenticonHandler> logger
        )
        {
            _esClient = esClient;
            _counterRepo = counterRepo;
            _logger = logger;
        }

        public async Task HandleAsync(
            ResolvedEvent resolvedEvent,
            NewCounterCreatedEvent e,
            CancellationToken cancellationToken
        )
        {
            var counter = e.Counter;

            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://identicon-api.herokuapp.com", UriKind.Absolute)
            };

            string requestUri = $"{counter.Id}/100?format=png";
            var httpResponse = await httpClient.GetAsync(
                requestUri,
                HttpCompletionOption.ResponseContentRead,
                cancellationToken
            ).ConfigureAwait(false);

            if (httpResponse.IsSuccessStatusCode)
            {
                byte[] content = await httpResponse.Content.ReadAsByteArrayAsync()
                    .ConfigureAwait(false);

                string pictureBase64 = Convert.ToBase64String(content);

                var error = await _counterRepo.SetPictureAsync(counter.Id, pictureBase64, cancellationToken)
                    .ConfigureAwait(false);

                if (error is null)
                {
                    try
                    {
                        await _esClient.AppendEventAsync(
                            resolvedEvent.Event.EventStreamId,
                            new CounterIdenticonGeneratedEvent
                            {
                                Picture = pictureBase64,
                            }
                        ).ConfigureAwait(false);
                    }
                    catch (Exception exception)
                    {
                        // ToDo
                        throw;
                    }
                }
                else
                {
                    _logger.LogError(
                        "Failed to set the picture for counter {counterId} due to {error}.",
                        counter.Id, error
                    );
                }
            }
            else
            {
                string content = await httpResponse.Content.ReadAsStringAsync()
                    .ConfigureAwait(false);

                _logger.LogError(
                    "Request {requestUri} got a response of {statusCode} with the content {content}.",
                    requestUri, httpResponse.StatusCode, content
                );
            }
        }
    }
}
