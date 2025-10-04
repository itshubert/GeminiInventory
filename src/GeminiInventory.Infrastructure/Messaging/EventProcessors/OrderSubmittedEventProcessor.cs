using GeminiInventory.Application.Common.Messaging;
using GeminiInventory.Application.Inventories.Commands;
using GeminiInventory.Infrastructure.Messaging.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GeminiInventory.Infrastructure.Messaging.EventProcessors;

public sealed class OrderSubmittedEventProcessor : IEventProcessor<OrderSubmittedEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<OrderSubmittedEventProcessor> _logger;

    public OrderSubmittedEventProcessor(ILogger<OrderSubmittedEventProcessor> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    public async Task ProcessEventAsync(OrderSubmittedEvent @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing OrderSubmitted event: {OrderId}", @event.Id);

        var reserveCommand = new ReserveInventoryForOrderCommand(
            OrderId: @event.Id,
            Items: @event.Items.Select(i => (i.ProductId, i.Quantity)));

        var result = await _mediator.Send(reserveCommand, cancellationToken);

        if (result.IsError)
        {
            _logger.LogWarning(
                "Failed to reserve inventory for order {OrderId}: {Errors}",
                @event.Id,
                string.Join(", ", result.Errors.Select(e => e.Description)));
            // Domain event (InventoryReservationFailedDomainEvent) will be published by the handler
            return;
        }

        _logger.LogInformation(
            "Successfully reserved inventory for order {OrderId}",
            @event.Id);
        // Domain event (InventoryReservedDomainEvent) will be published by SaveChanges via interceptor
    }
}