using System;
using System.Threading.Tasks;
using ESC.Data.Redis.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;

namespace ESC.Data.Redis
{
    public class CounterRepo : IDisposable
    {
        private readonly IDatabase _db;

        private readonly ConnectionMultiplexer _redisConnection;

        public CounterRepo()
        {
            _redisConnection = ConnectionMultiplexer.Connect("localhost");
            _db = _redisConnection.GetDatabase();
        }

        public Task SetCounterAsync(Counter counter)
        {
            string json = JsonConvert.SerializeObject(counter);
            return _db.StringSetAsync($"counter:{counter.Name}", json);
        }

        public async Task<Counter> GetCounterByNameAsync(string counterName)
        {
            string json = await _db.StringGetAsync($"counter:{counterName}")
                .ConfigureAwait(false);

            Counter counter = null;
            if (json != null)
            {
                counter = JsonConvert.DeserializeObject<Counter>(json);
            }

            return counter;
        }

        public Task SetEventPositionAsync(string position)
        {
            string json = JsonConvert.SerializeObject(new {position});
            return _db.StringSetAsync("last_event", json);
        }

        public Task<string> GetLastEventPositionAsync() =>
            _db.StringGetAsync("last_event")
                .ContinueWith(t =>
                    {
                        string json = t.Result;
                        return json == null
                            ? null
                            : JsonConvert.DeserializeObject<JObject>(t.Result).Value<string>("position");
                    },
                    TaskContinuationOptions.OnlyOnRanToCompletion
                );

        public void Dispose()
        {
            _redisConnection?.Dispose();
        }
    }
}