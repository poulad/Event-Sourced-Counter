using System;
using System.Threading.Tasks;
using ESC.Data.Redis;
using ESC.Web.Events;
using ESC.Web.Models;
using ESC.Web.Services;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> StartCounter(string name)
        {
            bool counterExists;
            {
                var counter = await _counterRepo.GetCounterByName(name)
                    .ConfigureAwait(false);
                counterExists = counter != null;
            }

            Result result;

            if (counterExists)
            {
                result = new Result(false, "Counter already exists");
            }
            else
            {
                await _eventsRepo.AppendCounterEventAsync(name, new SimpleEvent(EventTypes.CounterCreated));
                result = new Result(true);
            }

            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> QueryCounter(string name)
        {
            var counter = await _counterRepo.GetCounterByName(name)
                .ConfigureAwait(false);

            if (counter != null)
            {
                return Json(counter);
            }
            else
            {
                return Json(new Result(false, "Counter must be created first"));
            }
        }

        [HttpPatch]
        public async Task<IActionResult> IncrementCounter(string name, [FromQuery] int count = 1)
        {
            Result result;
            bool counterExists;
            {
                var counter = await _counterRepo.GetCounterByName(name)
                    .ConfigureAwait(false);
                counterExists = counter != null;
            }

            if (counterExists)
            {
                if (0 < count)
                {
                    await _eventsRepo.AppendCounterEventAsync(name, new CounterIncrementedEvent(count));
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