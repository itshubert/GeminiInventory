using ErrorOr;
using GeminiInventory.Domain.Common.Models;
using GeminiInventory.Domain.InventoryAggregate.Events;
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

    public void UpdateStockLevel(int newStockLevel)
    {
        QuantityAvailable = newStockLevel;
        UpdatedAt = DateTimeOffset.UtcNow;

        AddDomainEvent(new InventoryUpdatedDomainEvent(
            Id,
            ProductId,
            QuantityAvailable,
            QuantityReserved,
            LastRestockDate,
            MinimumStockLevel,
            UpdatedAt));
    }

    public void UpdateStock(int quantityAvailable, int quantityReserved, DateTimeOffset lastRestockDate, int minimumStockLevel)
    {
        QuantityAvailable = quantityAvailable;
        QuantityReserved = quantityReserved;
        LastRestockDate = lastRestockDate;
        MinimumStockLevel = minimumStockLevel;
        UpdatedAt = DateTimeOffset.UtcNow;

        AddDomainEvent(new InventoryUpdatedDomainEvent(
            Id,
            ProductId,
            QuantityAvailable,
            QuantityReserved,
            LastRestockDate,
            MinimumStockLevel,
            UpdatedAt));
    }

    public ErrorOr<Success> ReserveStock(Guid orderId, int quantity)
    {
        if (quantity <= 0)
        {
            return Error.Validation(
                code: "InvalidQuantity",
                description: "Quantity to reserve must be greater than zero.");
        }

        if (QuantityAvailable - quantity < 0)
        {
            // TODO: Raise InventoryUpdateFailedDomainEvent domain event then handler publishes to SQS OrderStockFailed event which is consumed by Order service to update order status to 'StockFailed'
            return Error.Failure(
                code: "InsufficientStock",
                description: "Not enough stock available to fulfill the reservation.");
        }

        QuantityAvailable -= quantity;
        QuantityReserved += quantity;
        UpdatedAt = DateTimeOffset.UtcNow;

        // Raise InventoryUpdatedDomainEvent to track inventory state changes
        AddDomainEvent(new InventoryUpdatedDomainEvent(
            Id,
            ProductId,
            QuantityAvailable,
            QuantityReserved,
            LastRestockDate,
            MinimumStockLevel,
            UpdatedAt));

        // Note: InventoryReservedDomainEvent is raised at the command handler level
        // after all items are reserved to ensure one event per order, not per item

        return new Success();
    }
}