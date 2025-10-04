using GeminiInventory.Application.Common.Messaging;
using GeminiInventory.Infrastructure.Messaging.Events;
using Microsoft.Extensions.Logging;

namespace GeminiInventory.Infrastructure.Messaging.EventProcessors;

public sealed class OrderSubmittedProcessor : IEventProcessor<OrderSubmitted>
{
    private readonly ILogger<OrderSubmittedProcessor> _logger;

    public OrderSubmittedProcessor(ILogger<OrderSubmittedProcessor> logger)
    {
        _logger = logger;
    }

    public async Task ProcessEventAsync(OrderSubmitted @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing OrderSubmitted event: {OrderId}", @event.OrderId);
        // Add your event processing logic here

        await ValueTask.CompletedTask;
    }
}