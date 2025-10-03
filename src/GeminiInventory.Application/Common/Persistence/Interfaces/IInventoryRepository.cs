using GeminiInventory.Domain.InventoryAggregate;

namespace GeminiInventory.Application.Common.Persistence.Interfaces;

public interface IInventoryRepository
{
    Task<IEnumerable<Inventory>> GetByProductsAsync(IEnumerable<Guid> productIds, CancellationToken cancellationToken);
    Task AddAsync(Inventory inventory, CancellationToken cancellationToken);
    Task<Inventory?> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken);
    Task<Inventory?> GetByProductIdForUpdateAsync(Guid productId, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}