using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ESC.Data.Redis;
using ESC.Events;
using ESC.Web.Models;
using ESC.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ESC.Web.Controllers
{
    [Route("api/counters/{name}")]
    public class CounterController : Controller
    {
        private readonly EventsRepo _eventsRepo;
        private readonly CounterRepo _counterRepo;

        public CounterController()
        {
            _eventsRepo = new EventsRepo();
            _counterRepo = new CounterRepo();
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewCounter(string name)
        {
            Result result;

            // validate the counter name user asked for
            bool isValidName = Regex.IsMatch(name, @"^[A-Z](?:[A-Z]|\d|-|_|\.)+$", RegexOptions.IgnoreCase);
            if (isValidName)
            {
                bool isDuplicateCounterName;
                {
                    var counter = await _counterRepo.GetCounterByNameAsync(name)
                        .ConfigureAwait(false);
                    isDuplicateCounterName = counter != null;
                }

                if (!isDuplicateCounterName)
                {
                    string reqId = Activity.Current?.Id ?? HttpContext.TraceIdentifier ?? Guid.NewGuid().ToString();

                    await _eventsRepo.AppendWebRequestEventAsync(
                        new NewCounterRequestedEvent
                        {
                            Name = name,
                            CorrelationId = reqId,
                        }
                    ).ConfigureAwait(false);

                    result = new Result(true, "Request scheduled. Your Counter will be created soon.", reqId);
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

            return Json(result, new JsonSerializerSettings {Formatting = Formatting.Indented});
        }

        [HttpGet]
        public async Task<IActionResult> QueryCounter(string name)
        {
            Result result;

            var counterEntity = await _counterRepo.GetCounterByNameAsync(name)
                .ConfigureAwait(false);

            if (counterEntity != null)
            {
                result = new Result(true, value: (CounterDto) counterEntity);
            }
            else
            {
                result = new Result(false, "Counter must be created first.");
            }

            return Json(result, new JsonSerializerSettings {Formatting = Formatting.Indented});
        }

        [HttpPatch]
        public async Task<IActionResult> IncrementCounter(string name, [FromQuery] int count = 1)
        {
            Result result;
            bool counterExists;
            {
                var counter = await _counterRepo.GetCounterByNameAsync(name)
                    .ConfigureAwait(false);
                counterExists = counter != null;
            }

            if (counterExists)
            {
                if (0 < count)
                {
//                    await _eventsRepo.AppendCounterEventAsync(name, new CounterIncrementedEvent(count));
                    result = new Result(true);
                }
                else
                {
                    result = new Result(false, "Invalid count");
                }
            }
            else
            {
                result = new Result(false, "Counter must be created first");
            }

            return Json(result);
        }

        [HttpDelete]
        public Task<IActionResult> DeleteCounter(string name)
        {
            throw new NotImplementedException();
        }
    }
}