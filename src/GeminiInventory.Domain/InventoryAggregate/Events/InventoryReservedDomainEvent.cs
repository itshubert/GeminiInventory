using GeminiInventory.Domain.Common.Models;

namespace GeminiInventory.Domain.InventoryAggregate.Events;

public sealed record InventoryReservedDomainEvent(
    Guid OrderId,
    ShippingAddress ShippingAddress,
    IEnumerable<InventoryItemReserved> Items) : IDomainEvent;

public sealed record InventoryItemReserved(
    Guid ProductId,
    int QuantityAvailable,
    int QuantityReserved,
    DateTimeOffset LastRestockDate,
    int MinimumStockLevel);

public sealed record ShippingAddress(
    string FirstName,
    string LastName,
    string AddressLine1,
    string AddressLine2,
    string City,
    string State,
    string PostCode,
    string Country);