using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Zabr.Craiglists.Crawler.Consumer.Services;
using Zabr.Craiglists.Crawler.Data;
using Zabr.Craiglists.Crawler.Data.Health;
using Zabr.Craiglists.Crawler.Data.Repositories;
using Zabr.Craiglists.Crawler.RabbitMq.Extensions;

namespace Zabr.Craiglists.Crawler.Consumer
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

            var dbConnectionString = configuration.GetConnectionString("DefaultConnection");
            var rabbitMqConfig = configuration.GetSection("RabbitMq");

            if (rabbitMqConfig == null)
            {
                throw new Exception("RabbitMq connection is null");
            }

            if (dbConnectionString == null)
            {
                throw new Exception("DB connection is null");
            }

            //Add Logging
            builder.Services.AddLogging(x => x.AddConsole());


            // Add database context.

            //builder.Services.AddEntityFrameworkMySQL()
            //    .AddDbContext<CraiglistsContext>((serviceProvider, options) =>
            //    {
            //        options.UseMySQL(dbConnectionString).UseInternalServiceProvider(serviceProvider);
            //    });

            builder.Services.AddDbContext<CraiglistsContext>(optionsBuilder =>
            {
                optionsBuilder.UseMySQL(dbConnectionString);
            });
            
            builder.Services.AddScoped<DbContext, CraiglistsContext>();
            builder.Services.AddTransient<IPageRepository, PageRepository>();

#if DEBUG
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                await Data.Migrator.Program.Main(new string[] { });
            }
#endif
            builder.Services.AddHttpClient("NoAutomaticCookies")
                .ConfigurePrimaryHttpMessageHandler(() =>
                    new HttpClientHandler
                    {
                        UseCookies = false
                    });
            
            builder.Services.AddTransient<CraiglistsDirectoryService>();
            
            builder.Services.AddRabbitMqConsumer(rabbitMqConfig);
            builder.Services.AddHostedService<ConsumerService>();

            var health = builder.Services.AddHealthChecks();

            health.AddCheck<DbContextHealthCheck<CraiglistsContext>>(
                name: "database",
                failureStatus: HealthStatus.Unhealthy,
                timeout: TimeSpan.FromSeconds(1),
                tags: new string[] { "service" });

            
            health.AddRabbitMQ($"amqp://{rabbitMqConfig["Username"]}:{rabbitMqConfig["Password"]}@{rabbitMqConfig["Host"]}:{rabbitMqConfig["Port"]}/{rabbitMqConfig["VirtualHost"]}",
                name: "rabbitmq",
                failureStatus: HealthStatus.Degraded,
                tags: new string[] { "service" });

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
