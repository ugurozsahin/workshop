using Entity;
using Microsoft.EntityFrameworkCore;

namespace Data.Context
{
    public class ContentContext : DbContext
    {
        public ContentContext(DbContextOptions<ContentContext> options) : base(options)
        {
            ChangeTracker.AutoDetectChangesEnabled = false;
        }

        public DbSet<Content> Content { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Content>()
                .ToTable("Content")
                .HasKey(content => content.Id);
        }
    }
}
