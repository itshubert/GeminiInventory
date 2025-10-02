using GeminiInventory.Domain.ReservationAggregate;

namespace GeminiInventory.Application.Common.Persistence.Interfaces;

public interface IReservationRepository
{
    Task AddAsync(Reservation reservation, CancellationToken cancellationToken = default);
    Task<IEnumerable<Reservation>> GetPendingReservationsByProductAsync(Guid productId, CancellationToken cancellationToken = default);
}