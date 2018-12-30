using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;

namespace ESC.Data.Redis
{
    public class ConfigRepo : IConfigRepository, IDisposable
    {
        private IDatabase _db;

        private ConnectionMultiplexer _redisConnection;

        public Task SetLastProcessedEventIdAsync(
            string stream,
            long position,
            CancellationToken cancellationToken = default
        )
        {
            EnsureConnected();
            string json = JsonConvert.SerializeObject(new { position });
            return _db.StringSetAsync($"last_event:{stream}", json);
        }

        public Task<long?> GetLastProcessedEventIdAsync(
            string stream,
            CancellationToken cancellationToken = default
        )
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
