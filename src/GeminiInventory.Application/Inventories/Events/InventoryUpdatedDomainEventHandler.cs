using System.Diagnostics;
using GeminiInventory.Domain.InventoryAggregate.Events;
using MediatR;

namespace GeminiInventory.Application.Inventories.Events;

public sealed class InventoryUpdatedDomainEventHandler : INotificationHandler<InventoryUpdatedDomainEvent>
{
    // private readonly IEventBridgePublisher _eventBridgePublisher;

    // public InventoryUpdatedDomainEventHandler(IEventBridgePublisher eventBridgePublisher)
    // {
    //     _eventBridgePublisher = eventBridgePublisher;
    // }

    public async Task Handle(InventoryUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var eventDetail = new
        {
            notification.InventoryId,
            notification.ProductId,
            notification.QuantityAvailable,
            notification.QuantityReserved,
            notification.LastRestockDate,
            notification.MinimumStockLevel,
            notification.UpdatedAt
        };

        Debug.WriteLine("InventoryUpdatedDomainEvent handled:");
        Debug.WriteLine(System.Text.Json.JsonSerializer.Serialize(eventDetail));

        await ValueTask.CompletedTask;

        // await _eventBridgePublisher.PublishEventAsync(
        //     source: "GeminiInventory",
        //     detailType: "InventoryUpdated",
        //     detail: eventDetail,
        //     cancellationToken: cancellationToken);
    }
}