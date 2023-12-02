using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Zabr.Craiglists.Crawler.Data.Entities;

namespace Zabr.Craiglists.Crawler.Data
{
    public class CraiglistsContext : DbContext
    {
        public virtual DbSet<PageEntity> Pages { get; set; }

        public CraiglistsContext(DbContextOptions<CraiglistsContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            if (connectionString != null)
            {
                optionsBuilder.UseMySQL(connectionString, builder =>
                {
                    builder.EnableRetryOnFailure(3);
                });
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PageEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Date).IsRequired();
                entity.Property(e => e.Url).IsRequired();
                entity.Property(e => e.Response).IsRequired();
            });
        }
    }
}
