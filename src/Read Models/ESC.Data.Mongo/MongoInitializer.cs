using System.Threading;
using System.Threading.Tasks;
using ESC.Data.Entities;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace ESC.Data.Mongo
{
    /// <summary>
    /// MongoDB initialization helper
    /// </summary>
    public static class MongoInitializer
    {
        /// <summary>
        /// Creates the database schema
        /// </summary>
        /// <param name="database">Database instance</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        public static async Task CreateSchemaAsync(
            IMongoDatabase database,
            CancellationToken cancellationToken = default
        )
        {
            {
                // "counters" Collection
                await database.CreateCollectionAsync("counters", cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
                var listsCollection = database.GetCollection<Counter>("counters");
                var indexBuilder = Builders<Counter>.IndexKeys;

                // create unique index "counter_name" on the field "name"
                await listsCollection.Indexes.CreateOneAsync(
                    new CreateIndexModel<Counter>(
                        indexBuilder.Ascending(c => c.Name),
                        new CreateIndexOptions { Name = "counter_name", Unique = true }
                    ), cancellationToken: cancellationToken
                ).ConfigureAwait(false);
            }

            {
                // "configs" Collection
                await database.CreateCollectionAsync("configs", cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Registers all the mappings between data entities and the documents stored in MongoDB collections
        /// </summary>
        public static void RegisterClassMaps()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(Counter)))
            {
                BsonClassMap.RegisterClassMap<Counter>(map =>
                {
                    map.MapIdProperty(c => c.Id);
                    map.MapProperty(c => c.Name).SetElementName("name").SetOrder(1);
                    map.MapProperty(c => c.Count).SetElementName("count").SetOrder(2);
                    map.MapProperty(c => c.Picture).SetElementName("picture").SetIgnoreIfDefault(true);
                    map.MapProperty(c => c.Version).SetElementName("v");
                    map.MapProperty(c => c.CreatedAt).SetElementName("created_at");
                    map.MapProperty(c => c.LastModifiedAt).SetElementName("last_modified_at").SetIgnoreIfDefault(true);
                });
            }
        }
    }
}
