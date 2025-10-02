using Microsoft.EntityFrameworkCore;

namespace GeminiInventory.Infrastructure.Persistence;

public sealed class GeminiInventoryDbContext : DbContext
{
    public GeminiInventoryDbContext(DbContextOptions<GeminiInventoryDbContext> options)
        : base(options)
    {
    }

    // Define DbSets for your entities here
    // public DbSet<Product> Products { get; set; }
    // public DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure your entity mappings here
        // modelBuilder.Entity<Product>(entity =>
        // {
        //     entity.ToTable("Products");
        //     entity.HasKey(e => e.Id);
        //     entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
        //     // Additional configurations...
        // });

        // modelBuilder.Entity<Category>(entity =>
        // {
        //     entity.ToTable("Categories");
        //     entity.HasKey(e => e.Id);
        //     entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
        //     // Additional configurations...
        // });
    }
}