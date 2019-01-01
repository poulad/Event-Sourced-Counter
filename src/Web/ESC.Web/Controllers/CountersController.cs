using System.Linq;
using System.Threading.Tasks;
using ESC.Data;
using ESC.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NUlid;

namespace ESC.Web.Controllers
{
    [Route("api/counters")]
    public class CountersController : Controller
    {
        private readonly ICounterRepository _counterRepo;

        public CountersController(
            ICounterRepository counterRepo
        )
        {
            _counterRepo = counterRepo;
        }

        [HttpGet]
        public async Task<IActionResult> QueryCounters(
            [FromQuery] string after = null,
            [FromQuery] int size = 10
        )
        {
            Result result;

            if (after is null ^ Ulid.TryParse(after, out _))
            {
                if (1 <= size && size <= 25)
                {
                    var countersResult = await _counterRepo.GetCountersInPageAsync(
                        after, size, HttpContext.RequestAborted
                    ).ConfigureAwait(false);

                    if (countersResult.Error is null)
                    {
                        result = new Result(
                            true,
                            value: countersResult.Counters.Select(c => (CounterDto) c)
                        );
                    }
                    else
                    {
                        result = new Result(false, "Failed to get the page.");
                    }
                }
                else
                {
                    result = new Result(
                        false,
                        "The value for the \"size\" parameter should be between 1 to 25."
                    );
                }
            }
            else
            {
                result = new Result(false, "Invalid ID provided for the \"after\" parameter.");
            }

            return Json(result, new JsonSerializerSettings { Formatting = Formatting.Indented });
        }
    }
}
