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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AdminUser>(entity =>
            {
                entity.HasIndex(u => u.Username).IsUnique();
            });
        }
    }
}


