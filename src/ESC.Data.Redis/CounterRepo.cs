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
        private IDatabase _db;

        private ConnectionMultiplexer _redisConnection;

        public Task SetCounterAsync(Counter counter)
        {
            EnsureConnected();
            string json = JsonConvert.SerializeObject(counter);
            return _db.StringSetAsync($"counter:{counter.Name}", json);
        }

        public async Task<Counter> GetCounterByNameAsync(string counterName)
        {
            EnsureConnected();
            string json = await _db.StringGetAsync($"counter:{counterName}")
                .ConfigureAwait(false);

            Counter counter = null;
            if (json != null)
            {
                counter = JsonConvert.DeserializeObject<Counter>(json);
            }

            return counter;
        }

        public Task SetLastProcessedEventIdAsync(string stream, long position)
        {
            EnsureConnected();
            string json = JsonConvert.SerializeObject(new {position});
            return _db.StringSetAsync($"last_event:{stream}", json);
        }

        public Task<long?> GetLastProcessedEventIdAsync(string stream)
        {
            EnsureConnected();
            return _db.StringGetAsync($"last_event:{stream}")
                .ContinueWith(t =>
                    {
                        string json = t.Result;
                        return json == null
                            ? null
                            : JsonConvert.DeserializeObject<JObject>(t.Result).Value<long?>("position");
                    },
                    TaskContinuationOptions.OnlyOnRanToCompletion
                );
        }

        private void EnsureConnected()
        {
            if (_redisConnection?.IsConnected != true)
            {
                _redisConnection = ConnectionMultiplexer.Connect("localhost");
                _db = _redisConnection.GetDatabase();
            }
        }

        public void Dispose()
        {
            _redisConnection?.Dispose();
        }
    }
}