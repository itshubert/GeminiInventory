using GeminiInventory.Application.Inventories.Commands;
using GeminiInventory.Application.Inventories.Queries;
using GeminiInventory.Contracts.Requests;
using GeminiInventory.Contracts.Responses;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GeminiInventory.Api.Controllers;

[Route("[controller]")]
public sealed class InventoryController : BaseController
{
    public InventoryController(ISender mediator, IMapper mapper) : base(mediator, mapper)
    {
    }

    [HttpPost("by-products")]
    public async Task<IActionResult> GetInventoriesByProducts([FromBody] ByProductIdsRequest request)
    {
        var result = await Mediator.Send(new GetInventoriesByProductsQuery(request.ProductIds));

        return Ok(Mapper.Map<IEnumerable<InventoryResponse>>(result));
    }

    [HttpPost("product/{productId}/adjust-stock-level/{stockLevel}")]
    public async Task<IActionResult> AdjustStock(Guid productId, int stockLevel)
    {
        var result = await Mediator.Send(new UpdateInventoryStockLevelCommand(productId, stockLevel));

        return result.Match(
            inventory => Ok(Mapper.Map<InventoryResponse>(inventory)),
            Problem
        );
    }
}