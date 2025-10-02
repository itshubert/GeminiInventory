using GeminiInventory.Domain.InventoryAggregate;
using GeminiInventory.Domain.ReservationAggregate;
using GeminiInventory.Infrastructure.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace GeminiInventory.Infrastructure.Persistence;

public sealed class GeminiInventoryDbContext : DbContext
{
    private readonly PublishDomainEventsInterceptor _publishDomainEventsInterceptor;

    public GeminiInventoryDbContext(DbContextOptions<GeminiInventoryDbContext> options, PublishDomainEventsInterceptor publishDomainEventsInterceptor)
        : base(options)
    {
        _publishDomainEventsInterceptor = publishDomainEventsInterceptor;
    }

    public DbSet<Inventory> Inventories { get; set; }
    public DbSet<Reservation> Reservations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GeminiInventoryDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_publishDomainEventsInterceptor);
        base.OnConfiguring(optionsBuilder);
    }
}