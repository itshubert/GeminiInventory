namespace GeminiInventory.Application.Common.Models.Inventories;

public sealed record InventoryModel(
    Guid ProductId,
    int QuantityAvailable,
    int QuantityReserved,
    DateTimeOffset LastRestockDate,
    int MinimumStockLevel,
    DateTimeOffset UpdatedAt);