using GeminiInventory.Application.Common.Models.Inventories;
using GeminiInventory.Application.Common.Persistence.Interfaces;
using MapsterMapper;
using MediatR;

namespace GeminiInventory.Application.Inventories.Queries;

public sealed record GetInventoriesByProductsQuery(IEnumerable<Guid> ProductIds) : IRequest<IEnumerable<InventoryModel>>;

public sealed class GetInventoriesByProductsQueryHandler : IRequestHandler<GetInventoriesByProductsQuery, IEnumerable<InventoryModel>>
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IMapper _mapper;

    public GetInventoriesByProductsQueryHandler(IInventoryRepository inventoryRepository, IMapper mapper)
    {
        _inventoryRepository = inventoryRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<InventoryModel>> Handle(GetInventoriesByProductsQuery request, CancellationToken cancellationToken)
    {
        var inventories = await _inventoryRepository.GetByProductsAsync(request.ProductIds, cancellationToken);
        return _mapper.Map<IEnumerable<InventoryModel>>(inventories);
    }
}