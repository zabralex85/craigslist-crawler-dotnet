using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Zabr.Crawler.Data.Migrator
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            if (args.Contains("--dryrun"))
            {
                return;
            }

            await MigrateDatabase();
        }

        private static async Task MigrateDatabase()
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ??
                                  throw new InvalidOperationException("ASPNETCORE_ENVIRONMENT in not set");

            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.{environmentName}.json")
                .AddEnvironmentVariables()
                .Build();

            var connectionString = config["DbSettings:MigrationConnectionString"];
            if (connectionString != null)
            {
                var migrationRunner = new MigratorRunner(connectionString);
                await migrationRunner.MigrateAsync();
            }
        }
    }

    internal class MigratorRunner
    {
        private readonly string _connectionString;

        public MigratorRunner(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task MigrateAsync()
        {
            await using var context = new DataDbContext(new DbContextOptions<DataDbContext>());
            context.Database.SetConnectionString(_connectionString);

            await context.Database.MigrateAsync();
        }
    }
}
