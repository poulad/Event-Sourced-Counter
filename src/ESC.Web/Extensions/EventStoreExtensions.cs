using System;
using ESC.Web.Options;
using ESC.Web.Services;
using EventStore.ClientAPI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ESC.Web.Extensions
{
    internal static class EventStoreExtensions
    {
        /// <summary>
        /// Adds Event Store services to the app's service collection
        /// </summary>
        public static void AddEventStore(
            this IServiceCollection services,
            IConfigurationSection dataSection
        )
        {
            string connectionString = dataSection.GetValue<string>(nameof(DataOptions.EventStoreConnectionString));
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException($@"Invalid Event Store connection string: ""{connectionString}"".");
            }

            services.Configure<DataOptions>(dataSection);

            services.AddScoped(provider =>
            {
                var dataOptions = provider.GetRequiredService<IOptions<DataOptions>>().Value;
                return EventStoreConnection.Create(dataOptions.EventStoreConnectionString);
            });

            services.AddTransient<IEventStoreClient, EventStoreClient>();
        }
    }
}
