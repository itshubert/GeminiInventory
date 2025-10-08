namespace GeminiInventory.Application.Common.Models.Inventories;

public sealed record InventoryReservedIntegrationModel(
    Guid InventoryId,
    Guid OrderId,
    ShippingAddressIntegrationModel ShippingAddress,
    IEnumerable<InventoryModel> Items);

public sealed record ShippingAddressIntegrationModel(
    string FirstName,
    string LastName,
    string AddressLine1,
    string AddressLine2,
    string City,
    string State,
    string PostalCode,
    string Country);