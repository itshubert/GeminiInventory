using GeminiInventory.Application.Common.Messaging;
using GeminiInventory.Application.Common.Models.Inventories;
using GeminiInventory.Domain.InventoryAggregate.Events;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GeminiInventory.Application.Inventories.Events;

public sealed class InventoryReservedDomainEventHandler : INotificationHandler<InventoryReservedDomainEvent>
{
    private readonly IEventBridgePublisher _eventBridgePublisher;
    private readonly IMapper _mapper;
    private readonly ILogger<InventoryReservedDomainEventHandler> _logger;

    public InventoryReservedDomainEventHandler(
        IEventBridgePublisher eventBridgePublisher,
        IMapper mapper,
        ILogger<InventoryReservedDomainEventHandler> logger)
    {
        _eventBridgePublisher = eventBridgePublisher;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task Handle(InventoryReservedDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling InventoryReservedDomainEvent for OrderId: {OrderId}", notification.OrderId);

        await _eventBridgePublisher.PublishAsync(
            detailType: "InventoryReserved",
            eventDetail: _mapper.Map<InventoryReservedIntegrationModel>(notification),
            cancellationToken: cancellationToken);
    }
}