using MediatR;
using AIIntegrator.Api.Contracts.Models;
using AIIntegrator.Application.Queries;
using System.Text.Json;

namespace AIIntegrator.Application.QueryHandlers;

public class GetContextQueryHandler : IRequestHandler<GetContextQuery, Context>
{
    public async Task<Context> Handle(GetContextQuery request, CancellationToken cancellationToken)
    {
        var fileName = $"{request.System}.json";

        if (!File.Exists(fileName))
        {
            throw new ArgumentException($"Context for {request.System} not found");
        }

        var json = await File.ReadAllTextAsync(fileName, cancellationToken);
        var context = JsonSerializer.Deserialize<Context>(json);
        return context;
    }
}
