using GeminiInventory.Domain.Common.Models;
using GeminiInventory.Domain.ReservationAggregate.ValueObjects;

namespace GeminiInventory.Domain.ReservationAggregate;

public sealed class Reservation : Entity<ReservationId>
{
    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public ReservationStatus Status { get; private set; }
    public DateTimeOffset ReservedAt { get; private set; }
    public DateTimeOffset? ExpiresAt { get; private set; }

    private Reservation(
        ReservationId id,
        Guid orderId,
        Guid productId,
        int quantity,
        ReservationStatus status,
        DateTimeOffset reservedAt) : base(id)
    {
        OrderId = orderId;
        ProductId = productId;
        Quantity = quantity;
        Status = status;
        ReservedAt = reservedAt;
    }

#pragma warning disable CS8618
    private Reservation() : base() { }
#pragma warning restore CS8618

    public static Reservation Create(
        Guid orderId,
        Guid productId,
        int quantity,
        ReservationStatus status,
        DateTimeOffset reservedAt)
    {
        return new(
            ReservationId.CreateUnique(),
            orderId,
            productId,
            quantity,
            status,
            reservedAt);
    }

    public void Expire()
    {
        Status = ReservationStatus.Expired;
        ExpiresAt = DateTimeOffset.UtcNow;
    }

    public void Complete()
    {
        Status = ReservationStatus.Confirmed;
        ExpiresAt = DateTimeOffset.UtcNow;
    }
}