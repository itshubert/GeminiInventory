using GeminiInventory.Application.Common.Messaging;
using GeminiInventory.Domain.InventoryAggregate.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GeminiInventory.Application.Inventories.Events;

public sealed class InventoryUpdatedDomainEventHandler : INotificationHandler<InventoryUpdatedDomainEvent>
{
    private readonly IEventBridgePublisher _eventBridgePublisher;
    private readonly ILogger<InventoryUpdatedDomainEventHandler> _logger;

    public InventoryUpdatedDomainEventHandler(
        IEventBridgePublisher eventBridgePublisher,
        ILogger<InventoryUpdatedDomainEventHandler> logger)
    {
        _eventBridgePublisher = eventBridgePublisher;
        _logger = logger;
    }

    public async Task Handle(InventoryUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var eventDetail = new
        {
            inventoryId = notification.InventoryId.Value,
            productId = notification.ProductId,
            quantityAvailable = notification.QuantityAvailable,
            quantityReserved = notification.QuantityReserved,
            lastRestockDate = notification.LastRestockDate,
            minimumStockLevel = notification.MinimumStockLevel,
            updatedAt = notification.UpdatedAt
        };

        _logger.LogInformation(
            "Publishing InventoryLevelChanged event for ProductId: {ProductId}, QuantityAvailable: {QuantityAvailable}",
            notification.ProductId,
            notification.QuantityAvailable);

        await _eventBridgePublisher.PublishAsync(
            detailType: "InventoryLevelChanged",
            eventDetail: eventDetail,
            cancellationToken: cancellationToken);
    }
}