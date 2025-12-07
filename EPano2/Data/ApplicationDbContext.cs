using EPano2.Models;
using Microsoft.EntityFrameworkCore;

namespace EPano2.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<AdminUser> AdminUsers => Set<AdminUser>();
        public DbSet<VideoConfig> VideoConfigs => Set<VideoConfig>();
        public DbSet<ExtraVideoLink> ExtraVideoLinks => Set<ExtraVideoLink>();
        public DbSet<TickerConfig> TickerConfigs => Set<TickerConfig>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AdminUser>(entity =>
            {
                entity.HasIndex(u => u.Username).IsUnique();
            });

            modelBuilder.Entity<VideoConfig>(entity =>
            {
                // İsteğe bağlı: varsayılan tek kayıt
                entity.HasMany(v => v.ExtraVideoLinks)
                      .WithOne(e => e.VideoConfig)
                      .HasForeignKey(e => e.VideoConfigId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}



