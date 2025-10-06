using GeminiInventory.Application.Common.Messaging;
using GeminiInventory.Domain.InventoryAggregate.Events;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GeminiInventory.Application.Inventories.Events;

public sealed class OrderStockFailedDomainEventHandler : INotificationHandler<OrderStockFailedDomainEvent>
{
    private readonly IEventBridgePublisher _eventBridgePublisher;
    private readonly IMapper _mapper;
    private readonly ILogger<OrderStockFailedDomainEventHandler> _logger;

    public OrderStockFailedDomainEventHandler(
        IEventBridgePublisher eventBridgePublisher,
        IMapper mapper,
        ILogger<OrderStockFailedDomainEventHandler> logger)
    {
        _eventBridgePublisher = eventBridgePublisher;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task Handle(OrderStockFailedDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling OrderStockFailedDomainEvent for OrderId: {OrderId}", notification.OrderId);

        await _eventBridgePublisher.PublishAsync(
            detailType: "OrderStockFailed",
            eventDetail: _mapper.Map<OrderStockFailedIntegrationModel>(notification),
            cancellationToken: cancellationToken);
    }
}