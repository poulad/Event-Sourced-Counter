using System;
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
            await _eventsRepo.AppendCounterEventAsync(name, new SimpleEvent(EventTypes.CounterCreated));

            return Json(new Result(true));
        }

        [HttpGet]
        public async Task<IActionResult> QueryCounter(string name)
        {
            var events = await _eventsRepo.GetAllCounterEventsAsync(name);

            var counter = new Counter {Name = name};
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
            if (0 < count)
            {
                await _eventsRepo.AppendCounterEventAsync(name, new CounterIncrementedEvent(count));
                result = new Result(true);
            }
            else
            {
                result = new Result(false, "Invalid count");
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