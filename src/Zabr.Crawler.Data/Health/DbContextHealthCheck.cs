using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Zabr.Crawler.Data.Repositories;

namespace Zabr.Crawler.Data.Health
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class DbContextHealthCheck<TContext>
        : IHealthCheck where TContext : DbContext
    {
        private readonly DbContext _dbContext = null!;
        private readonly IServiceProvider _serviceProvider = null!;

        // ReSharper disable once UnusedMember.Global
        public DbContextHealthCheck(TContext dbContext, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
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
                using (var scope = _serviceProvider.CreateScope())
                {
                    var respository = (PageRepository)scope.ServiceProvider.GetRequiredService<IPageRepository>();

                    await _dbContext.Database.CanConnectAsync(cancellationToken);
                    return HealthCheckResult.Healthy();
                }
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("DbContext connection test failed", ex);
            }
        }
    }
}
