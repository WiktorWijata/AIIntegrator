using Microsoft.AspNetCore.Mvc;
using AIIntegrator.Api.Contracts.Models;
using AIIntegrator.Application.Commands;
using AIIntegrator.Application.Queries;
using MediatR;

namespace AIIntegrator.Api.Controllers;

[ApiController]
[Route("context")]
public class ContextController : ControllerBase
{
    private readonly IMediator mediator;

    public ContextController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpPost("set-context")]
    public async Task<IActionResult> SetContextAsync([FromHeader(Name = "X-System")] string system, [FromBody] Context context)
    {
        await this.mediator.Send(new SetContextCommand(system, context));
        return this.NoContent();
    }

    [HttpGet("get")]
    public async Task<IActionResult> GetContextAsync([FromHeader(Name = "X-System")] string system)
    {
        var context = await this.mediator.Send(new GetContextQuery(system));
        return this.Ok(context);
    }
}
