namespace GeminiInventory.Domain.InventoryAggregate.Events;

public sealed record OrderStockFailedIntegrationModel(
    Guid OrderId,
    IEnumerable<LineItemStockItemFailed> FailedItems);

public sealed record LineItemStockItemFailedIntegrationModel(
Guid ProductId,
int QuantityAvailable,
string Reason);