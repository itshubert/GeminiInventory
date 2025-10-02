using GeminiInventory.Application.Common.Persistence.Interfaces;
using GeminiInventory.Domain.ReservationAggregate;
using Microsoft.EntityFrameworkCore;

namespace GeminiInventory.Infrastructure.Persistence.Repositories;

public sealed class ReservationRepository : BaseRepository, IReservationRepository
{
    public ReservationRepository(GeminiInventoryDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Reservation>> GetPendingReservationsByProductAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await _context.Reservations
            .Where(r => r.ProductId == productId && r.Status == ReservationStatus.Pending)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Reservation reservation, CancellationToken cancellationToken = default)
    {
        await _context.Reservations.AddAsync(reservation, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}