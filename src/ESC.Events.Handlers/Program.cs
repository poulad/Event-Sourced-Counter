using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ESC.Events.Handlers
{
    class Program
    {
        static async Task Main()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonEnvVar("ESC_HANDLERS_SETTINGS", optional: true)
                .Build();

            var serviceCollection = new ServiceCollection()
                .AddLogging(builder => builder
                    .AddConfiguration(configuration.GetSection("Logging"))
                    .AddConsole()
                );

            var startup = new Startup(configuration);
            startup.ConfigureServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            ILogger logger;
            {
                var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
                logger = loggerFactory.CreateLogger<Program>();
            }
            logger.LogInformation("Starting the application.");

            await startup.RunAsync(serviceProvider)
                .ConfigureAwait(false);

            logger.LogInformation("Application is stopped.");
        }
    }
}
