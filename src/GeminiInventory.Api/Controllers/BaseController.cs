using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GeminiInventory.Api.Controllers;

[ApiController]
public abstract class BaseController : ApiController
{
    protected readonly ISender Mediator;
    protected readonly IMapper Mapper;

    public BaseController(ISender mediator, IMapper mapper)
    {
        Mediator = mediator;
        Mapper = mapper;
    }
}