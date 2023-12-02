using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Zabr.Crawler.Data.Entities;

namespace Zabr.Crawler.Data
{
    public class DataDbContext : DbContext
    {
        public virtual DbSet<PageEntity> Pages { get; set; }

        public DataDbContext(DbContextOptions<DataDbContext> options) : base(options)
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
                entity.Property(e => e.Response).IsRequired();
                entity.Property(e => e.Url)
                    .HasColumnType("NVARCHAR")
                    .HasMaxLength(250)
                    .IsRequired();

                entity.HasIndex(x => x.Url, "IX_URL").IsUnique();
            });
        }
    }
}
