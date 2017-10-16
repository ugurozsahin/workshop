using Entity;
using Microsoft.EntityFrameworkCore;

namespace Data.Context
{
    public class ProductModelContext : DbContext
    {
        public ProductModelContext(DbContextOptions<ProductModelContext> options) : base(options)
        {
            ChangeTracker.AutoDetectChangesEnabled = false;
        }

        public DbSet<ProductModel> Content { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductModel>()
                .ToTable("ProductModel")
                .HasKey(content => content.Id);
        }
    }
}
