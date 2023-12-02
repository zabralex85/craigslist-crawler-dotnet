using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Zabr.Craiglists.Crawler.Producer.Services;
using Zabr.Craiglists.Crawler.RabbitMq.Extensions;

namespace Zabr.Craiglists.Crawler.Producer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var configuration = builder.Configuration; // allows both to access and to set up the config
            var environment = builder.Environment;

            configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            configuration.AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true);
            configuration.AddEnvironmentVariables();

            var rabbitMqConfig = configuration.GetSection("RabbitMq");
            if (rabbitMqConfig == null)
            {
                throw new Exception("RabbitMq connection is null");
            }

            //Add Logging
            builder.Services.AddLogging(x => x.AddConsole());

            //builder.Services.Configure<RabbitMqOptions>(rabbitMqConfig);

            builder.Services.AddRabbitMqProducer(rabbitMqConfig);
            builder.Services.AddHostedService<WorkerService>();

            var health = builder.Services.AddHealthChecks();

            var app = builder.Build();

            app.MapHealthChecks("/health", new HealthCheckOptions
            {
                ResultStatusCodes =
                {
                    [HealthStatus.Healthy] = StatusCodes.Status200OK,
                    [HealthStatus.Degraded] = StatusCodes.Status200OK,
                    [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
                }
            });

            app.Logger.LogInformation("Starting Application");
            await app.RunAsync();
        }
    }
}
