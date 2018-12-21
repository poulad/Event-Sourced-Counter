using System.Threading.Tasks;
using EventSourcedCounter.Events;
using EventSourcedCounter.Models;
using EventSourcedCounter.Services;
using Microsoft.AspNetCore.Mvc;

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
                if (ev.EventType == EventTypes.CounterCreated)
                {
                    counter.LastModifiedAt = ev.Created;
                }
            }

            return Json(counter);
        }

        [HttpPatch]
        public async Task<IActionResult> IncrementCounter(string name)
        {
            return Json(new { });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCounter(string name)
        {
            return Json(new { });
        }
    }
}