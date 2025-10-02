namespace GeminiInventory.Contracts.Responses;

public sealed record InventoryResponse(
    Guid ProductId,
    int QuantityAvailable,
    int QuantityReserved,
    DateTimeOffset LastRestockDate,
    int MinimumStockLevel,
    DateTimeOffset UpdatedAt);