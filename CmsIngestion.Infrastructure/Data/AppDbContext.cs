using CmsIngestion.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace CmsIngestion.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<CmsEntity> CmsEntities => Set<CmsEntity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CmsEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PayloadJson).IsRequired();
                entity.Property(e => e.Version).IsRequired();
                entity.Property(e => e.IsDisabled).HasDefaultValue(false);

                // Indexing ID for high-performance webhook lookups and upserts
                entity.HasIndex(e => e.Id).IsUnique();
            });
        }
    }
}
