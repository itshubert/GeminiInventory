using GeminiInventory.Domain.InventoryAggregate;

namespace GeminiInventory.Application.Common.Persistence.Interfaces;

public interface IInventoryRepository
{
    Task AddAsync(Inventory inventory, CancellationToken cancellationToken);
    Task<Inventory?> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken);
}