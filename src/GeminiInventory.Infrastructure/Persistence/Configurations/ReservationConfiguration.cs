using GeminiInventory.Domain.ReservationAggregate;
using GeminiInventory.Domain.ReservationAggregate.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeminiInventory.Infrastructure.Persistence.Configurations;

public sealed class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.ToTable("Reservations", "public");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id)
            .ValueGeneratedNever()
            .HasConversion(
                id => id.Value,
                value => ReservationId.Create(value));

        builder.Property(r => r.OrderId)
            .IsRequired();
        // Note: No FK to Inventory here

        builder.Property(r => r.ProductId)
            .IsRequired();
        // This could have FK to Product table if it exists

        builder.Property(r => r.Quantity)
            .IsRequired();

        builder.Property(r => r.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(r => r.ReservedAt)
            .IsRequired();

        builder.Property(r => r.ExpiresAt);

        // Indexes for performance
        builder.HasIndex(r => r.OrderId);
        builder.HasIndex(r => r.ProductId);
        builder.HasIndex(r => new { r.ProductId, r.Status });
    }
}