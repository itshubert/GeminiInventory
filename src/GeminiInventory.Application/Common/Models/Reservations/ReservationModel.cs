namespace GeminiInventory.Application.Common.Models.Reservations;

public sealed record ReservationModel(
    Guid Id,
    Guid OrderId,
    Guid ProductId,
    int Quantity,
    string Status,
    DateTimeOffset ReservedAt,
    DateTimeOffset? ExpiresAt);