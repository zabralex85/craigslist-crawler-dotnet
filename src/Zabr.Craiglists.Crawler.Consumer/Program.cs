using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Zabr.Craiglists.Crawler.Data;
using Zabr.Craiglists.Crawler.Data.Health;
using Zabr.Craiglists.Crawler.Data.Repositories;

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


            //Add Logging
            builder.Services.AddLogging(x => x.AddConsole());

            // Add database context.
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            if (connectionString != null)
            {
                builder.Services.AddDbContext<CraiglistsContext>(optionsBuilder =>
                {
                    optionsBuilder.UseMySQL(connectionString);
                });

#if DEBUG
                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                {
                    await Data.Migrator.Program.Main(new string[]{});
                }
#endif
                builder.Services.AddScoped<IPageRepository, PageRepository>();
            }
            else
            {

            }

            var health = builder.Services.AddHealthChecks();
            health.AddCheck<DbContextHealthCheck<CraiglistsContext>>("DatabaseCheck", HealthStatus.Unhealthy, timeout: TimeSpan.FromSeconds(1));
            
            //builder.Services.AddControllers();
            //builder.Services.AddEndpointsApiExplorer();
            //builder.Services.AddSwaggerGen();

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

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapDefaultControllerRoute();
            //    endpoints.MapControllerRoute(
            //        name: "Default",
            //        pattern: "{controller=Home}/{action=Index}"
            //    );
            //});

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
            //    app.UseSwagger();
            //    app.UseSwaggerUI();
            //}

            //app.UseAuthorization();
            //app.MapControllers();

            app.Logger.LogInformation("Starting Application");
            await app.RunAsync();
        }
    }
}
