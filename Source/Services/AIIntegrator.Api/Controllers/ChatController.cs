using Microsoft.AspNetCore.Mvc;
using AIIntegrator.Application.Commands;
using MediatR;
using AIIntegrator.Api.Contracts.Models;

namespace AIIntegrator.Api.Controllers;

[ApiController]
[Route("chat")]
public class ChatController : ControllerBase
{
    private readonly IMediator mediator;

    public ChatController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpPost("send-message")]

    public async Task<IActionResult> SendAsync([FromHeader(Name = "X-System")] string system, [FromBody] ChatMessage message)
    {
        var response = await this.mediator.Send(new SendChatMessageCommand(system, message));
        return this.Ok(response);
    }
}
