using GeminiInventory.Domain.Common.Models;
using GeminiInventory.Domain.InventoryAggregate.ValueObjects;

namespace GeminiInventory.Domain.InventoryAggregate.Events;

public sealed record InventoryReservedDomainEvent(
    InventoryId InventoryId,
    Guid OrderId,
    IEnumerable<InventoryItemReserved> Items) : IDomainEvent;

public sealed record InventoryItemReserved(
    Guid ProductId,
    int QuantityAvailable,
    int QuantityReserved,
    DateTimeOffset LastRestockDate,
    int MinimumStockLevel);