using System;
using ESC.Data;
using ESC.Data.Entities;
using ESC.Data.Mongo;
using ESC.Events.Handlers.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;

namespace ESC.Events.Handlers.Extensions
{
    internal static class MongoDbExtensions
    {
        /// <summary>
        /// Adds MongoDB services to the app's service collection
        /// </summary>
        public static void AddMongoDb(
            this IServiceCollection services,
            IConfigurationSection dataSection
        )
        {
            string connectionString = dataSection.GetValue<string>(nameof(DataOptions.MongoConnectionString));
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException($@"Invalid MongoDB connection string: ""{connectionString}"".");
            }

            services.Configure<DataOptions>(dataSection);

            string dbName = new ConnectionString(connectionString).DatabaseName;
            services.AddSingleton<IMongoClient, MongoClient>(_ => new MongoClient(connectionString));
            services.AddTransient(provider =>
                provider.GetRequiredService<IMongoClient>().GetDatabase(dbName)
            );

            services.AddTransient<ICounterRepository, CounterRepo>();
            services.AddTransient(provider =>
                provider.GetRequiredService<IMongoDatabase>().GetCollection<Counter>("counters")
            );

            services.AddTransient<IConfigRepository, ConfigRepo>();
            services.AddTransient(provider =>
                provider.GetRequiredService<IMongoDatabase>().GetCollection<BsonDocument>("configs")
            );

            MongoInitializer.RegisterClassMaps();
        }

        public static void CreateMongoDbSchema(
            this IServiceScope scope
        )
        {
            var db = scope.ServiceProvider.GetRequiredService<IMongoDatabase>();

            var collection = db.ListCollections(
                new ListCollectionsOptions
                {
                    Filter = Builders<BsonDocument>.Filter.Eq("name", "counters")
                }
            ).FirstOrDefault();

            if (collection is null)
            {
                MongoInitializer.CreateSchemaAsync(db).GetAwaiter().GetResult();
            }
        }
    }
}
