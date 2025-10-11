namespace GeminiInventory.Application.Common.Models.Inventories;

public sealed record InventoryReservedIntegrationModel(
    Guid OrderId,
    ShippingAddressIntegrationModel ShippingAddress,
    IEnumerable<InventoryModel> Items);

// TODO: Remove ShippingAddress, OrderFulfillment gets this from OrderSubmitted event
public sealed record ShippingAddressIntegrationModel(
    string FirstName,
    string LastName,
    string AddressLine1,
    string AddressLine2,
    string City,
    string State,
    string PostCode,
    string Country);