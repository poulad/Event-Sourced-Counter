using System;
using System.Threading;
using System.Threading.Tasks;
using ESC.Data.Entities;
using StackExchange.Redis;

namespace ESC.Data.Redis
{
    public class CounterRepo : ICounterRepository, IDisposable
    {
        private IDatabase _db;

        private ConnectionMultiplexer _redisConnection;

        public Task<Error> AddAsync(
            Counter counter,
            CancellationToken cancellationToken = default
        )
        {
            throw new NotImplementedException();
//            EnsureConnected();
//            string json = JsonConvert.SerializeObject(counter);
//            return _db.StringSetAsync($"counter:{counter.Name}", json);
        }

        public Task<(Counter Counter, Error Error)> GetByIdAsync(
            string id,
            CancellationToken cancellationToken = default
        )
        {
            throw new NotImplementedException();
        }

        public Task<(Counter Counter, Error Error)> GetByNameAsync(
            string name,
            CancellationToken cancellationToken = default
        )
        {
            throw new NotImplementedException();
//            EnsureConnected();
//            string json = await _db.StringGetAsync($"counter:{name}")
//                .ConfigureAwait(false);
//
//            Counter counter = null;
//            if (json != null)
//            {
//                counter = JsonConvert.DeserializeObject<Counter>(json);
//            }
//
//            return counter;
        }

        public Task<(Counter[] Counters, Error Error)> GetCountersInPageAsync(
            string afterId = null,
            int pageSize = 10,
            CancellationToken cancellationToken = default
        )
        {
            throw new NotImplementedException();
        }

        public Task<Error> SetPictureAsync(
            string id,
            string pictureBase64,
            CancellationToken cancellationToken = default
        )
        {
            throw new NotImplementedException();
        }

        public Task<Error> DeleteAsync(
            string id,
            CancellationToken cancellationToken = default
        )
        {
            throw new NotImplementedException();
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
