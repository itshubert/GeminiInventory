using GeminiInventory.Domain.Common.Models;

namespace GeminiInventory.Domain.InventoryAggregate.Events;

public sealed record OrderStockFailedDomainEvent(
    Guid OrderId,
    IEnumerable<LineItemStockItemFailed> FailedItems) : IDomainEvent;

public sealed record LineItemStockItemFailed(
    Guid ProductId,
    string Reason);
