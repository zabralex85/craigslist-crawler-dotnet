using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Zabr.Craiglists.Crawler.Data.Health
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class DbContextHealthCheck<TContext>
        : IHealthCheck where TContext : DbContext
    {
        private readonly DbContext _dbContext = null!;

        // ReSharper disable once UnusedMember.Global
        public DbContextHealthCheck(TContext dbContext)
        {
            _dbContext = dbContext;
        }

        // ReSharper disable once UnusedMember.Global
        public DbContextHealthCheck()
        {
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await _dbContext.Database.CanConnectAsync(cancellationToken);
                return HealthCheckResult.Healthy();
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("DbContext connection test failed", ex);
            }
        }
    }
}
