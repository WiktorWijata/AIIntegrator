using System.Text.Json;
using System.Text.RegularExpressions;
using System.Reflection;
using Microsoft.Extensions.Options;
using AIIntegrator.Api.Contracts.ReadModels;
using AIIntegrator.Application.Commands;
using AIIntegrator.Application.Queries;
using AIIntegrator.Application.Templates;
using AIIntegrator.ExternalContracts;
using AIIntegrator.ExternalContracts.Models.Request;
using AIIntegrator.ExternalContracts.Models.Response;
using MediatR;
using Polly;
using Message = AIIntegrator.ExternalContracts.Models.Request.Message;

namespace AIIntegrator.Application.CommandHandlers;

public class SendChatMessageCommandHandler : IRequestHandler<SendChatMessageCommand, ChatResponseReadModel>
{
    private readonly IMediator mediator;
    private readonly AISettings aiSettings;
    private readonly IOpenRouterApi openRouterApi;
    private readonly IAsyncPolicy<ChatResponse> retryPolicy;

    public SendChatMessageCommandHandler(IMediator mediator, IOptions<AISettings> options, IOpenRouterApi openRouterApi)
    {
        this.mediator = mediator;
        this.openRouterApi = openRouterApi;
        this.aiSettings = options.Value;
        this.retryPolicy = Policy<ChatResponse>
            .Handle<Exception>()
            .OrResult(response => response == null)
            .WaitAndRetryForeverAsync(
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(2),
                onRetry: (exception, timespan, context) =>
                {
                    Console.WriteLine($"Ponawiam próbę... Czas: {DateTime.Now}");
                })
            .WrapAsync(Policy.TimeoutAsync<ChatResponse>(
                timeout: TimeSpan.FromMinutes(2),
                onTimeoutAsync: async (context, timespan, task) =>
                {
                    Console.WriteLine("Przekroczono limit czasu 2 minut.");
                }));
    }

    public async Task<ChatResponseReadModel> Handle(SendChatMessageCommand request, CancellationToken cancellationToken)
    {
        var prompt = await this.mediator.Send(new GetPromptQuery(request.System));
        var chatRequest = new ChatRequest()
        {
            Model = this.aiSettings.Model,
            Messages = new Message[]
            {
                new Message()
                {
                    Role =  Roles.System.ToString().ToLowerInvariant(),
                    Content = prompt
                },
                new Message()
                {
                    Role = Roles.User.ToString().ToLowerInvariant(),
                    Content = request.Message.Message
                }
            }
        };

        if (request.Message.Context != null)
        {
            chatRequest.Messages = chatRequest.Messages.Concat(new Message[]
            {
                new Message()
                {
                    Role = Roles.User.ToString().ToLowerInvariant(),
                    Content = this.GetAdditionalContext(request.Message.Context)
                }
            }).ToArray();
        }

        var chatResponse = await this.retryPolicy.ExecuteAsync(async () => await this.openRouterApi.SendMessageAsync(chatRequest));
        if (chatResponse != null)
        {
            var content = chatResponse.Choices[0].Message.Content;
            var json = this.ExtractJson(content);
            var result = JsonSerializer.Deserialize<ChatResponseReadModel>(json, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            result.UserMessage = request.Message.Message;
            return result;
        }

        throw new TimeoutException("Przekroczono limit czasu 2 minut.");
    }

    private string GetAdditionalContext(object additionalContext)
    {
        var assembly = Assembly.GetExecutingAssembly();
        using (var stream = assembly.GetManifestResourceStream(ConstsTemplates.AdditionalContextTemplate))
        {
            if (stream == null)
            {
                throw new FileNotFoundException("Resource not found", ConstsTemplates.AdditionalContextTemplate);
            }

            using (var reader = new StreamReader(stream))
            {
                var template = reader.ReadToEnd();
                var context = template.Replace(ConstsMnemonics.AdditionalContext, JsonSerializer.Serialize(additionalContext));
                return context;
            }
        }
    }

    private string ExtractJson(string response)
    {
        var match = Regex.Match(response, @"\{.*\}", RegexOptions.Singleline);
        return match.Success ? match.Value : "{}";
    }
}
