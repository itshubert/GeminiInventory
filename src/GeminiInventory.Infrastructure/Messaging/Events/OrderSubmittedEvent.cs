namespace GeminiInventory.Infrastructure.Messaging.Events;

public sealed class OrderSubmittedEvent
{
    public Guid Id { get; init; }
    public Guid CustomerId { get; init; }
    public DateTimeOffset OrderDate { get; init; }
    public string Status { get; init; } = string.Empty;
    public decimal TotalAmount { get; init; }
    public string Currency { get; init; } = string.Empty;
    public IEnumerable<OrderItem> Items { get; init; } = Array.Empty<OrderItem>();

}

public sealed record OrderItem(
    Guid Id,
    Guid ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal SubTotal);
