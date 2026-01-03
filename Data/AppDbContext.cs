using Microsoft.EntityFrameworkCore;
using SimpleShopApi.Data.Entities;

namespace SimpleShopApi.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options):DbContext(options)
    {
        public DbSet<ApiKey> ApiKeys { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApiKey>(e =>
            {
                e.HasKey(e => e.Id);
                e.Property(e => e.CreatedAt).ValueGeneratedOnAdd();
            });
            base.OnModelCreating(modelBuilder);
        }
    }
}
