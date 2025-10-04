namespace GeminiInventory.Application.Common.Models.Inventories;

public sealed record InventoryReservedIntegrationModel(
    Guid InventoryId,
    Guid OrderId,
    IEnumerable<InventoryModel> Items);