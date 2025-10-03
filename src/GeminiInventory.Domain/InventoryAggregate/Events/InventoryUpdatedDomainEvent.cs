using GeminiInventory.Domain.Common.Models;
using GeminiInventory.Domain.InventoryAggregate.ValueObjects;

namespace GeminiInventory.Domain.InventoryAggregate.Events;

public sealed record InventoryUpdatedDomainEvent(
    InventoryId InventoryId,
    Guid ProductId,
    int QuantityAvailable,
    int QuantityReserved,
    DateTimeOffset LastRestockDate,
    int MinimumStockLevel,
    DateTimeOffset UpdatedAt) : IDomainEvent;