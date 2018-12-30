using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ESC.Data.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ESC.Data.Mongo
{
    public class CounterRepo : ICounterRepository
    {
        private readonly IMongoCollection<Counter> _collection;

        private FilterDefinitionBuilder<Counter> Filter => Builders<Counter>.Filter;

        /// <inheritdoc />
        public CounterRepo(
            IMongoCollection<Counter> collection
        )
        {
            _collection = collection;
        }

        /// <inheritdoc />
        public async Task<Error> AddAsync(
            Counter counter,
            CancellationToken cancellationToken = default
        )
        {
            Error error;
            try
            {
                await _collection.InsertOneAsync(counter, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

                error = null;
            }
            catch (MongoWriteException e) when (
                e.WriteError.Category == ServerErrorCategory.DuplicateKey
            )
            {
                Dictionary<string, object> errorItems = null;
                if (e.WriteError.Message.Contains(" index: counter_name "))
                {
                    errorItems = new Dictionary<string, object> { { "key", nameof(Counter.Name) } };
                }

                error = new Error(
                    ErrorCodes.Duplicate,
                    items: errorItems
                );
            }

            return error;
        }

        /// <inheritdoc />
        public async Task<(Counter Counter, Error Error)> GetByIdAsync(
            string id,
            CancellationToken cancellationToken = default
        )
        {
            Counter entity = await _collection
                .Find(Filter.Eq("_id", id))
                .SingleOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);
            if (entity is null)
            {
                throw new Exception(id); // ToDo
            }

            return (entity, null); // ToDo
        }

        /// <inheritdoc />
        public async Task<(Counter Counter, Error Error)> GetByNameAsync(
            string name,
            CancellationToken cancellationToken = default
        )
        {
            name = Regex.Escape(name);
            var filter = Filter.Regex(c => c.Name, new BsonRegularExpression($"^{name}$", "i"));

            Counter entity = await _collection
                .Find(filter)
                .SingleOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            Error error = entity is null
                ? new Error(ErrorCodes.NotFound)
                : null;

            return (entity, error);
        }

        /// <inheritdoc />
        public async Task<(Counter[] Counters, Error Error)> GetCountersInPageAsync(
            string afterId,
            int pageSize,
            CancellationToken cancellationToken = default
        )
        {
            (Counter[] Counters, Error Error) result;

            var findTask = _collection
                .Find(
                    afterId is null
                        ? FilterDefinition<Counter>.Empty
                        : Filter.Gt("_id", afterId)
                )
                .Sort(
                    Builders<Counter>.Sort.Ascending("_id")
                )
                .Limit(pageSize)
                .ToListAsync(cancellationToken);

            try
            {
                var entities = await findTask.ConfigureAwait(false);
                result = (entities.ToArray(), null);
            }
            catch (Exception e)
            {
                // ToDo return error
                result = (null, new Error("e.data")); // ToDo
            }

            return result;
        }

        public async Task<Error> SetPictureAsync(
            string id,
            string pictureBase64,
            CancellationToken cancellationToken = default
        )
        {
            Counter entity;
            try
            {
                entity = await _collection
                    .FindOneAndUpdateAsync(
                        Filter.Eq("_id", id),
                        Builders<Counter>.Update.Set(c => c.Picture, pictureBase64),
                        new FindOneAndUpdateOptions<Counter>
                        {
                            IsUpsert = false,
                        },
                        cancellationToken
                    ).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                // ToDo
                throw;
            }

            var error = entity is null
                ? new Error(ErrorCodes.NotFound)
                : null;

            return error;
        }

        /// <inheritdoc />
        public async Task<Error> DeleteAsync(
            string id,
            CancellationToken cancellationToken = default
        )
        {
            var result = await _collection
                .DeleteOneAsync(Filter.Eq("_id", ObjectId.Parse(id)), cancellationToken)
                .ConfigureAwait(false);
            if (result.DeletedCount == 0)
            {
                throw new Exception(); // ToDo
            }

            return null; // ToDo
        }
    }
}
