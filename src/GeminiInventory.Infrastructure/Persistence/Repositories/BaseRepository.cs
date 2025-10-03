namespace GeminiInventory.Infrastructure.Persistence.Repositories;

public abstract class BaseRepository
{
    protected readonly GeminiInventoryDbContext _context;

    protected BaseRepository(GeminiInventoryDbContext context)
    {
        _context = context;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}