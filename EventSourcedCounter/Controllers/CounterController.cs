using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventSourcedCounter.Events;
using EventSourcedCounter.Models;
using EventSourcedCounter.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EventSourcedCounter.Controllers
{
    [Route("api/counters/{name}")]
    public class CounterController : Controller
    {
        private readonly EventsRepo _eventsRepo;

        public CounterController()
        {
            _eventsRepo = new EventsRepo();
        }

        [HttpPost]
        public async Task<IActionResult> StartCounter(string name)
        {
            bool counterExists = await _eventsRepo.CounterExistsAsync(name);
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
            var events = await _eventsRepo.GetAllCounterEventsAsync(name);

            if (!events.Any())
            {
                return Json(new Result(false, "Counter must be created first"));
            }

            var counter = new Counter {Name = name};
            // aggregation of the events to build the current state:
            foreach (var ev in events)
            {
                if (ev.EventType == EventTypes.CounterIncremented)
                {
                    string json = Encoding.UTF8.GetString(ev.Data);
                    var jObj = JsonConvert.DeserializeObject<JObject>(json);

                    counter.Value += jObj.Value<int>("count");
                }
                else if (ev.EventType == EventTypes.CounterCreated)
                {
                    counter.CreatedAt = ev.Created;
                }

                counter.LastModifiedAt = ev.Created;
            }

            return Json(counter);
        }

        [HttpPatch]
        public async Task<IActionResult> IncrementCounter(string name, [FromQuery] int count = 1)
        {
            Result result;
            bool counterExists = await _eventsRepo.CounterExistsAsync(name);

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