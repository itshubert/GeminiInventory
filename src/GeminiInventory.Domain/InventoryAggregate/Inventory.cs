using GeminiInventory.Domain.Common.Models;
using GeminiInventory.Domain.InventoryAggregate.ValueObjects;

namespace GeminiInventory.Domain.InventoryAggregate;

public sealed class Inventory : AggregateRoot<InventoryId>
{
    public Guid ProductId { get; private set; }
    public int QuantityAvailable { get; private set; }
    public int QuantityReserved { get; private set; }
    public DateTimeOffset LastRestockDate { get; private set; }
    public int MinimumStockLevel { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; } = DateTimeOffset.UtcNow;

    private Inventory(
        InventoryId id,
        Guid productId,
        int quantityAvailable,
        int quantityReserved,
        DateTimeOffset lastRestockDate,
        int minimumStockLevel) : base(id)
    {
        ProductId = productId;
        QuantityAvailable = quantityAvailable;
        QuantityReserved = quantityReserved;
        LastRestockDate = lastRestockDate;
        MinimumStockLevel = minimumStockLevel;
    }

#pragma warning disable CS8618
    private Inventory() : base() { }
#pragma warning restore CS8618

    public static Inventory Create(
        Guid productId,
        int quantityAvailable,
        int quantityReserved,
        DateTimeOffset lastRestockDate,
        int minimumStockLevel)
    {
        return new(
            InventoryId.CreateUnique(),
            productId,
            quantityAvailable,
            quantityReserved,
            lastRestockDate,
            minimumStockLevel);
    }

    public void UpdateStock(int quantityAvailable, int quantityReserved, DateTimeOffset lastRestockDate, int minimumStockLevel)
    {
        QuantityAvailable = quantityAvailable;
        QuantityReserved = quantityReserved;
        LastRestockDate = lastRestockDate;
        MinimumStockLevel = minimumStockLevel;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

}