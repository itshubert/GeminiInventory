using ErrorOr;
using GeminiInventory.Application.Common.Persistence.Interfaces;
using GeminiInventory.Domain.InventoryAggregate.Events;
using GeminiInventory.Domain.InventoryAggregate.ValueObjects;
using MapsterMapper;
using MediatR;

namespace GeminiInventory.Application.Inventories.Commands;

public sealed record ReserveInventoryForOrderCommand(
    Guid OrderId,
    IEnumerable<(Guid ProductId, int Quantity)> Items) : IRequest<ErrorOr<Unit>>;

public sealed class ReserveInventoryForCommandHandler : IRequestHandler<ReserveInventoryForOrderCommand, ErrorOr<Unit>>
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IReservationRepository _reservationRepository;
    private readonly IPublisher _publisher;

    public ReserveInventoryForCommandHandler(
        IInventoryRepository inventoryRepository,
        IReservationRepository reservationRepository,
        IPublisher publisher)
    {
        _inventoryRepository = inventoryRepository;
        _reservationRepository = reservationRepository;
        _publisher = publisher;
    }

    public async Task<ErrorOr<Unit>> Handle(ReserveInventoryForOrderCommand request, CancellationToken cancellationToken)
    {
        var errors = new List<Error>();
        var reservedItems = new List<InventoryItemReserved>();

        await _inventoryRepository.BeginTransactionAsync(cancellationToken);

        try
        {
            // First, validate and reserve all items
            foreach (var item in request.Items)
            {
                var inventory = await _inventoryRepository.GetByProductIdForUpdateAsync(item.ProductId, cancellationToken);
                if (inventory is null)
                {
                    errors.Add(Error.Failure(
                        code: "Inventory.NotFound",
                        description: $"No inventory found for ProductId: {item.ProductId}"));
                    continue;
                }

                if (inventory.QuantityAvailable < item.Quantity)
                {
                    errors.Add(Error.Failure(
                        code: "Inventory.InsufficientStock",
                        description: $"Insufficient stock for ProductId: {item.ProductId}. Requested: {item.Quantity}, Available: {inventory.QuantityAvailable}"));
                    continue;
                }

                // Reserve the inventory
                var result = inventory.ReserveStock(request.OrderId, item.Quantity);
                if (result.IsError)
                {
                    errors.AddRange(result.Errors);
                    continue;
                }

                // Track successfully reserved items
                reservedItems.Add(new InventoryItemReserved(
                    inventory.ProductId,
                    inventory.QuantityAvailable,
                    inventory.QuantityReserved,
                    inventory.LastRestockDate,
                    inventory.MinimumStockLevel));
            }

            // If there were any errors, return them without saving
            if (errors.Any())
            {
                return errors;
            }

            // All items successfully reserved - raise ONE domain event for the entire order
            var inventoryReservedEvent = new InventoryReservedDomainEvent(
                InventoryId.CreateUnique(), // This represents the aggregate event, not a specific inventory
                request.OrderId,
                reservedItems);

            // Publish the domain event
            await _publisher.Publish(inventoryReservedEvent, cancellationToken);

            // Save all changes in one transaction
            await _inventoryRepository.SaveChangesAsync(cancellationToken);
            await _inventoryRepository.CommitTransactionAsync(cancellationToken);

            return Unit.Value;

        }
        catch
        {
            await _inventoryRepository.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}