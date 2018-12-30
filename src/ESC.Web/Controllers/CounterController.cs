using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ESC.Data;
using ESC.Events;
using ESC.Web.Models;
using ESC.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NUlid;

namespace ESC.Web.Controllers
{
    [Route("api/counters/{name}")]
    public class CounterController : Controller
    {
        private readonly IEventStoreClient _eventStoreClient;
        private readonly ICounterRepository _counterRepo;

        public CounterController(
            IEventStoreClient eventStoreClient,
            ICounterRepository counterRepo
        )
        {
            _counterRepo = counterRepo;
            _eventStoreClient = eventStoreClient;
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewCounter([FromRoute] string name)
        {
            Result result;

            // validate the counter name user asked for
            bool isValidName = Regex.IsMatch(
                name, @"^[A-Z](?:[A-Z]|\d|-|_|\.)+$", RegexOptions.IgnoreCase
            );
            if (isValidName)
            {
                var counterResult = await _counterRepo.GetByNameAsync(name)
                    .ConfigureAwait(false);

                if (counterResult.Error != null)
                {
                    if (counterResult.Error.Code == ErrorCodes.NotFound)
                    {
                        string correlationId = Ulid.NewUlid().ToString();

                        await _eventStoreClient.AppendMutationRequestEventAsync(
                            new NewCounterRequestedEvent
                            {
                                Name = name,
                                CorrelationId = correlationId,
                            }
                        ).ConfigureAwait(false);

                        result = new Result(
                            true,
                            "Request scheduled. Counter will be created soon.",
                            correlationId: correlationId
                        );
                    }
                    else
                    {
                        result = new Result(
                            false,
                            $"Failed to validate counter name. {counterResult.Error.Message}"
                        );
                    }
                }
                else
                {
                    result = new Result(false, "Counter with that name already exists.");
                }
            }
            else
            {
                result = new Result(
                    false,
                    "Counter name is invalid. " +
                    "Allowed characters are alphanumeric characters, dot, underscore, and hyphen. " +
                    "Name must start with a letter."
                );
            }

            return Json(result, new JsonSerializerSettings { Formatting = Formatting.Indented });
        }

        [HttpGet]
        public async Task<IActionResult> QueryCounter([FromRoute] string name)
        {
            Result result;

            var counterResult = await _counterRepo.GetByNameAsync(name)
                .ConfigureAwait(false);

            if (counterResult.Error is null)
            {
                result = new Result(true, value: (CounterDto) counterResult.Counter);
            }
            else
            {
                result = new Result(false, "Counter must be created first.");
            }

            return Json(result, new JsonSerializerSettings { Formatting = Formatting.Indented });
        }

        [HttpPatch]
        public async Task<IActionResult> IncrementCounter([FromRoute] string name, [FromQuery] int count = 1)
        {
            Result result;

            var counterResult = await _counterRepo.GetByNameAsync(name)
                .ConfigureAwait(false);

            if (counterResult.Error is null)
            {
                if (0 < count)
                {
                    string correlationId = Ulid.NewUlid().ToString();
                    await _eventStoreClient.AppendMutationRequestEventAsync(new CounterIncrementRequestedEvent
                    {
                        CounterName = name,
                        By = count,
                        CorrelationId = correlationId,
                    }).ConfigureAwait(false);

                    result = new Result(
                        true, "Request scheduled. Counter will be incremented soon.", correlationId: correlationId
                    );
                }
                else
                {
                    result = new Result(false, "Count value should be greater than zero.");
                }
            }
            else if (counterResult.Error.Code == ErrorCodes.NotFound)
            {
                result = new Result(false, "Counter must be created first.");
            }
            else
            {
                result = new Result(
                    false,
                    $"Failed to validate counter name. {counterResult.Error.Message}"
                );
            }

            return Json(result, new JsonSerializerSettings { Formatting = Formatting.Indented });
        }

        [HttpPut]
        public Task<IActionResult> SetCounterValue([FromRoute] string name, [FromBody] SetCountInputDto dto)
        {
            throw new NotImplementedException();
        }

        [HttpDelete]
        public Task<IActionResult> DeleteCounter([FromRoute] string name)
        {
            throw new NotImplementedException();
        }
    }
}
