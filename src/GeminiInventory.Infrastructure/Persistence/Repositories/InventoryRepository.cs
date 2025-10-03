using GeminiInventory.Application.Common.Persistence.Interfaces;
using GeminiInventory.Domain.InventoryAggregate;
using Microsoft.EntityFrameworkCore;

namespace GeminiInventory.Infrastructure.Persistence.Repositories;

public sealed class InventoryRepository : BaseRepository, IInventoryRepository
{
    public InventoryRepository(GeminiInventoryDbContext context) : base(context)
    {
    }

    public async Task<Inventory?> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken)
    {
        return await _context.Inventories
            .AsNoTracking()
            .Where(i => i.ProductId == productId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Inventory?> GetByProductIdForUpdateAsync(Guid productId, CancellationToken cancellationToken)
    {
        return await _context.Inventories
            .Where(i => i.ProductId == productId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<Inventory>> GetByProductsAsync(IEnumerable<Guid> productIds, CancellationToken cancellationToken)
    {
        return await _context.Inventories
            .AsNoTracking()
            .Where(i => productIds.Contains(i.ProductId))
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Inventory inventory, CancellationToken cancellationToken)
    {
        await _context.Inventories.AddAsync(inventory, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}