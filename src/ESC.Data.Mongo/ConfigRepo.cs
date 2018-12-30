using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ESC.Data.Mongo
{
    public class ConfigRepo : IConfigRepository
    {
        private readonly IMongoCollection<BsonDocument> _collection;

        private FilterDefinitionBuilder<BsonDocument> Filter => Builders<BsonDocument>.Filter;

        /// <inheritdoc />
        public ConfigRepo(
            IMongoCollection<BsonDocument> collection
        )
        {
            _collection = collection;
        }

        public async Task SetLastProcessedEventIdAsync(
            string stream,
            long position,
            CancellationToken cancellationToken = default
        )
        {
            var filter = Filter.Eq("stream", stream);
            var update = Builders<BsonDocument>.Update.Set("position", position);

            try
            {
                await _collection.UpdateOneAsync(
                    filter,
                    update,
                    new UpdateOptions { IsUpsert = true, },
                    cancellationToken
                ).ConfigureAwait(false);
            }
            catch (MongoWriteException e)
            {
                throw; // ToDo
            }
        }

        public async Task<long?> GetLastProcessedEventIdAsync(
            string stream,
            CancellationToken cancellationToken = default
        )
        {
            long? position;

            var document = await _collection
                .Find(Filter.Eq("stream", stream))
                .SingleOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            if (document != null)
            {
                position = document.GetElement("position").Value.AsInt64;
            }
            else
            {
                position = null;
            }

            return position;
        }
    }
}
