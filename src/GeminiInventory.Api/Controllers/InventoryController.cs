using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GeminiInventory.Api.Controllers;

[ApiController]
public sealed class InventoryController : BaseController
{
    public InventoryController(ISender mediator, IMapper mapper) : base(mediator, mapper)
    {
    }
}