using AIIntegrator.Api.Contracts.Models;
using AIIntegrator.Api.Contracts.ReadModels;
using MediatR;

namespace AIIntegrator.Application.Commands;

public class SendChatMessageCommand : IRequest<ChatResponseReadModel>
{
    public SendChatMessageCommand(string system, ChatMessage message)
    {
        this.System = system;
        this.Message = message;
    }

    public string System { get; set; }
    public ChatMessage Message { get; set; }
}
