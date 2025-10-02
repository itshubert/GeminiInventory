using GeminiInventory.Domain.InventoryAggregate;
using GeminiInventory.Domain.InventoryAggregate.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeminiInventory.Infrastructure.Persistence.Configurations;

public sealed class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
{
    public void Configure(EntityTypeBuilder<Inventory> builder)
    {
        builder.ToTable("Inventories", "public");

        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id)
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => InventoryId.Create(value));

        builder.Property(i => i.ProductId)
            .IsRequired();

        builder.Property(i => i.QuantityAvailable)
            .IsRequired();

        builder.Property(i => i.QuantityReserved)
            .IsRequired();

        builder.Property(i => i.LastRestockDate)
            .IsRequired();

        builder.Property(i => i.MinimumStockLevel)
            .IsRequired();

        builder.Property(i => i.UpdatedAt)
            .IsRequired();

        builder.HasIndex(i => i.ProductId).IsUnique();

        // Configure concurrency token
        builder.Property(i => i.UpdatedAt)
            .IsConcurrencyToken();
    }
}