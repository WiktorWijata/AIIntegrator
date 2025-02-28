using AIIntegrator.Application.Queries;
using MediatR;

namespace AIIntegrator.Application.QueryHandlers;

public class GetPromptQueryHandler : IRequestHandler<GetPromptQuery, string>
{
    public async Task<string> Handle(GetPromptQuery request, CancellationToken cancellationToken)
    {
        var promptFileName = Path.Combine("Contexts", request.System, "context.txt");

        if (!File.Exists(promptFileName))
        {
            throw new FileNotFoundException($"Prompt file for {request.System} not found");
        }

        var prompt = await File.ReadAllTextAsync(promptFileName, cancellationToken);
        return prompt;
    }
}
