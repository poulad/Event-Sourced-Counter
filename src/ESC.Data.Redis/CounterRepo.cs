using System;
using System.Threading.Tasks;
using ESC.Data.Redis.Entities;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace ESC.Data.Redis
{
    public class CounterRepo : IDisposable
    {
        private readonly ConnectionMultiplexer _redis;

        public CounterRepo()
        {
            _redis = ConnectionMultiplexer.Connect("localhost");
        }

        public async Task SetCounter(Counter counter)
        {
            var db = _redis.GetDatabase();

            string json = JsonConvert.SerializeObject(counter);
            await db.StringSetAsync($"counter:{counter.Name}", json)
                .ConfigureAwait(false);
        }

        public async Task<Counter> GetCounterByName(string counterName)
        {
            var db = _redis.GetDatabase();

            string json = await db.StringGetAsync($"counter:{counterName}")
                .ConfigureAwait(false);

            Counter counter = null;
            if (json != null)
            {
                counter = JsonConvert.DeserializeObject<Counter>(json);
            }

            return counter;
        }

        public void Dispose()
        {
            _redis?.Dispose();
        }
    }
}